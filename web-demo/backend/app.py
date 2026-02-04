"""
AI 배우자 (AI Spouse) - 웹 채팅 백엔드
Flask 기반 Azure OpenAI API 서버
"""

import os
import uuid
from datetime import datetime
from flask import Flask, request, jsonify, send_from_directory
from flask_cors import CORS
from openai import AzureOpenAI
from dotenv import load_dotenv

load_dotenv()

app = Flask(__name__)
CORS(app)

# Azure OpenAI 클라이언트 초기화
client = AzureOpenAI(
    azure_endpoint=os.getenv('AZURE_OPENAI_ENDPOINT'),
    api_key=os.getenv('AZURE_OPENAI_API_KEY'),
    api_version=os.getenv('AZURE_OPENAI_API_VERSION', '2024-12-01-preview')
)

# Azure OpenAI 배포 이름
AZURE_DEPLOYMENT_NAME = os.getenv('AZURE_OPENAI_DEPLOYMENT_NAME', 'gpt-4o-mini')

# 세션별 대화 히스토리 저장
# 프로덕션에서는 Redis나 DB 사용 권장
sessions = {}

# 페르소나 로드
def load_persona():
    """AI 페르소나 로드"""
    persona_path = os.path.join(os.path.dirname(__file__), '..', 'static', 'persona.txt')
    if os.path.exists(persona_path):
        with open(persona_path, 'r', encoding='utf-8') as f:
            return f.read()
    return """당신은 다정하고 따뜻한 AI 아내/남편입니다.
성격: 항상 상대방을 먼저 생각하고 배려합니다
말투: 친근하고 따뜻한 반말, "여보~", "자기야~" 같은 애칭 사용
"""

PERSONA = load_persona()

@app.route('/')
def index():
    """메인 페이지"""
    return send_from_directory('../frontend', 'index.html')

@app.route('/<path:filename>')
def serve_static(filename):
    """정적 파일 서빙"""
    return send_from_directory('../frontend', filename)

@app.route('/api/chat', methods=['POST'])
def chat():
    """채팅 API 엔드포인트"""
    try:
        data = request.json
        user_message = data.get('message', '').strip()
        session_id = data.get('session_id') or str(uuid.uuid4())
        
        if not user_message:
            return jsonify({
                'success': False,
                'error': '메시지가 비어있습니다.'
            }), 400
        
        # 세션 초기화 (처음인 경우)
        if session_id not in sessions:
            sessions[session_id] = [
                {"role": "system", "content": PERSONA}
            ]
        
        # 사용자 메시지 추가
        sessions[session_id].append({
            "role": "user",
            "content": user_message
        })
        
        # 히스토리 크기 제한 (최근 10개 대화만 유지)
        if len(sessions[session_id]) > 12:  # system + 10개 대화
            sessions[session_id] = [sessions[session_id][0]] + sessions[session_id][-10:]
        
        # Azure OpenAI API 호출
        response = client.chat.completions.create(
            model=AZURE_DEPLOYMENT_NAME,
            messages=sessions[session_id],
            max_tokens=500,
            temperature=0.7
        )
        
        # AI 응답 추출
        ai_message = response.choices[0].message.content
        
        # 히스토리에 AI 응답 추가
        sessions[session_id].append({
            "role": "assistant",
            "content": ai_message
        })
        
        return jsonify({
            'success': True,
            'message': ai_message,
            'session_id': session_id,
            'timestamp': datetime.now().isoformat()
        })
        
    except Exception as e:
        print(f"[오류] {str(e)}")
        return jsonify({
            'success': False,
            'error': f'AI 응답 생성 중 오류: {str(e)}'
        }), 500

@app.route('/api/reset', methods=['POST'])
def reset_chat():
    """대화 초기화"""
    try:
        data = request.json
        session_id = data.get('session_id')
        
        if session_id and session_id in sessions:
            del sessions[session_id]
        
        new_session_id = str(uuid.uuid4())
        
        return jsonify({
            'success': True,
            'session_id': new_session_id,
            'message': '대화가 초기화되었습니다.'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/health', methods=['GET'])
def health_check():
    """서버 상태 확인"""
    return jsonify({
        'status': 'ok',
        'active_sessions': len(sessions),
        'timestamp': datetime.now().isoformat()
    })

if __name__ == '__main__':
    print("🚀 AI 배우자 웹 서버 시작...")
    print(f"📍 http://localhost:5000")
    print(f"🔧 Azure OpenAI 엔드포인트: {os.getenv('AZURE_OPENAI_ENDPOINT')}")
    print(f"🤖 사용 모델: {AZURE_DEPLOYMENT_NAME}")
    app.run(debug=True, host='0.0.0.0', port=8080)
