# AI 배우자 (AI Spouse) - 웹 데모

Unity 없이 웹 브라우저에서 AI 배우자와 대화할 수 있는 웹 데모입니다.

## 🚀 빠른 시작

### 1. 환경 설정

```bash
# 1. 프로젝트 폴더로 이동
cd web-demo

# 2. Python 가상환경 생성
python -m venv venv

# 3. 가상환경 활성화
# Windows:
venv\Scripts\activate
# macOS/Linux:
source venv/bin/activate

# 4. 필요한 패키지 설치
pip install -r requirements.txt
```

### 2. OpenAI API 키 설정

`.env` 파일을 생성하고 API 키를 입력하세요:

```
OPENAI_API_KEY=your-api-key-here
```

또는 환경 변수로 설정:
```bash
# Windows
set OPENAI_API_KEY=your-api-key-here

# macOS/Linux
export OPENAI_API_KEY=your-api-key-here
```

### 3. 서버 실행

```bash
cd backend
python app.py
```

### 4. 브라우저에서 접속

브라우저에서 `http://localhost:5000` 으로 접속하세요.

## 📁 프로젝트 구조

```
web-demo/
├── backend/
│   └── app.py              # Flask 서버 + OpenAI API
├── frontend/
│   ├── index.html          # 메인 페이지
│   ├── style.css           # 스타일링
│   └── chat.js             # 채팅 로직
├── static/
│   └── persona.txt         # AI 페르소나 설정
└── requirements.txt        # Python 패키지 목록
```

## 🎨 기능

- 💬 실시간 채팅 (WebSocket 지원)
- 📱 모바일 친화적 UI
- 🎭 다정한 AI 배우자 페르소나
- 🔄 대화 히스토리 유지 (세션 기반)
- ⚡ 로딩 애니메이션

## 🌐 배포 방법

### 무료 호스팅 옵션

**Render.com:**
1. https://render.com 에 가입
2. GitHub 레포지토리 연결
3. `backend/app.py`를 시작 파일로 설정
4. 배포 완료!

**PythonAnywhere:**
1. https://pythonanywhere.com 에 가입
2. 파일 업로드
3. Flask 앱 설정

## ⚠️ 주의사항

- API 키는 절대로 GitHub에 커밋하지 마세요 (`.env` 파일은 `.gitignore`에 추가)
- API 호출 비용이 발생하므로 테스트 시 주의하세요

## 📚 참고

이 웹 데모는 Unity 프로젝트의 1단계 프로토타입과 동일한 기능을 제공합니다.
