/**
 * AI 배우자 채팅 클라이언트
 */

// 설정
const CONFIG = {
    API_URL: 'http://localhost:8080',  // Flask 서버 주소
    MAX_RETRY: 3,
    RETRY_DELAY: 1000
};

// 상태 관리
let sessionId = localStorage.getItem('ai_spouse_session_id') || null;
let isProcessing = false;
let messageHistory = [];

// DOM 요소
const elements = {
    messages: document.getElementById('messages'),
    input: document.getElementById('message-input'),
    sendBtn: document.getElementById('send-btn'),
    loading: document.getElementById('loading'),
    modal: document.getElementById('api-key-modal'),
    apiKeyInput: document.getElementById('api-key-input')
};

/**
 * 초기화
 */
document.addEventListener('DOMContentLoaded', () => {
    // 이벤트 리스너 설정
    elements.input.addEventListener('keypress', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    });
    
    // 자동 포커스
    elements.input.focus();
    
    // 서버 연결 확인
    checkServerHealth();
    
    console.log('🚀 AI 배우자 채팅 클라이언트 초기화 완료');
});

/**
 * 메시지 전송
 */
async function sendMessage() {
    const message = elements.input.value.trim();
    
    if (!message || isProcessing) {
        return;
    }
    
    // 입력 초기화
    elements.input.value = '';
    elements.input.focus();
    
    // 사용자 메시지 표시
    addMessage(message, 'user');
    
    // 처리 중 상태
    setProcessing(true);
    
    try {
        const response = await fetch(`${CONFIG.API_URL}/api/chat`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                message: message,
                session_id: sessionId
            })
        });
        
        const data = await response.json();
        
        if (data.success) {
            // 세션 ID 저장
            if (data.session_id) {
                sessionId = data.session_id;
                localStorage.setItem('ai_spouse_session_id', sessionId);
            }
            
            // AI 응답 표시
            addMessage(data.message, 'ai');
            
            // 히스토리에 추가
            messageHistory.push({ role: 'user', content: message });
            messageHistory.push({ role: 'ai', content: data.message });
        } else {
            // 오류 메시지 표시
            addSystemMessage(`⚠️ 오류: ${data.error || '알 수 없는 오류'}`);
            console.error('API 오류:', data.error);
        }
        
    } catch (error) {
        console.error('네트워크 오류:', error);
        addSystemMessage('⚠️ 서버 연결 오류. 잠시 후 다시 시도해주세요.');
    } finally {
        setProcessing(false);
    }
}

/**
 * 메시지 추가 (UI)
 */
function addMessage(text, sender) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${sender}-message`;
    
    const bubble = document.createElement('div');
    bubble.className = 'message-bubble';
    bubble.textContent = text;
    
    const time = document.createElement('div');
    time.className = 'message-time';
    time.textContent = getCurrentTime();
    
    messageDiv.appendChild(bubble);
    messageDiv.appendChild(time);
    
    elements.messages.appendChild(messageDiv);
    
    // 스크롤을 최하단으로
    scrollToBottom();
}

/**
 * 시스템 메시지 추가
 */
function addSystemMessage(text) {
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message ai-message';
    messageDiv.style.alignSelf = 'center';
    
    const bubble = document.createElement('div');
    bubble.className = 'message-bubble';
    bubble.style.background = '#ffebee';
    bubble.style.color = '#c62828';
    bubble.style.fontSize = '13px';
    bubble.textContent = text;
    
    messageDiv.appendChild(bubble);
    elements.messages.appendChild(messageDiv);
    
    scrollToBottom();
}

/**
 * 대화 초기화
 */
async function resetChat() {
    if (!confirm('대화를 초기화하시겠습니까? 모든 대화 기록이 삭제됩니다.')) {
        return;
    }
    
    try {
        const response = await fetch(`${CONFIG.API_URL}/api/reset`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                session_id: sessionId
            })
        });
        
        const data = await response.json();
        
        if (data.success) {
            // UI 초기화
            elements.messages.innerHTML = `
                <div class="message ai-message">
                    <div class="message-bubble">
                        안녕하세요, 여보~! 대화가 초기화되었어. 다시 이야기하자~ ♡
                    </div>
                    <div class="message-time">${getCurrentTime()}</div>
                </div>
            `;
            
            // 세션 ID 업데이트
            sessionId = data.session_id;
            localStorage.setItem('ai_spouse_session_id', sessionId);
            messageHistory = [];
            
            console.log('✅ 대화 초기화 완료');
        }
        
    } catch (error) {
        console.error('초기화 오류:', error);
        addSystemMessage('⚠️ 초기화 중 오류가 발생했습니다.');
    }
}

/**
 * 처리 중 상태 설정
 */
function setProcessing(processing) {
    isProcessing = processing;
    
    elements.sendBtn.disabled = processing;
    elements.input.disabled = processing;
    
    if (processing) {
        elements.loading.classList.remove('hidden');
    } else {
        elements.loading.classList.add('hidden');
    }
    
    scrollToBottom();
}

/**
 * 스크롤을 최하단으로
 */
function scrollToBottom() {
    const chatContainer = document.querySelector('.chat-container');
    chatContainer.scrollTop = chatContainer.scrollHeight;
}

/**
 * 현재 시간 포맷팅
 */
function getCurrentTime() {
    const now = new Date();
    const hours = now.getHours().toString().padStart(2, '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

/**
 * 서버 상태 확인
 */
async function checkServerHealth() {
    try {
        const response = await fetch(`${CONFIG.API_URL}/api/health`);
        const data = await response.json();
        
        if (data.status === 'ok') {
            console.log('✅ 서버 연결 정상');
            console.log(`📊 활성 세션: ${data.active_sessions}`);
        }
    } catch (error) {
        console.warn('⚠️ 서버 연결 실패. 로컬 모드로 실행 중일 수 있습니다.');
        console.log('💡 팁: 백엔드 서버를 먼저 실행하세요 (python backend/app.py)');
    }
}

/**
 * API 키 모달 열기
 */
function showApiKeyModal() {
    elements.modal.classList.remove('hidden');
    elements.apiKeyInput.focus();
}

/**
 * API 키 모달 닫기
 */
function closeModal() {
    elements.modal.classList.add('hidden');
}

/**
 * API 키 저장
 */
function saveApiKey() {
    const apiKey = elements.apiKeyInput.value.trim();
    
    if (apiKey) {
        localStorage.setItem('openai_api_key', apiKey);
        closeModal();
        addSystemMessage('✅ API 키가 저장되었습니다.');
    } else {
        alert('API 키를 입력해주세요.');
    }
}

/**
 * 로컬 저장소에서 API 키 로드
 */
function loadApiKey() {
    return localStorage.getItem('openai_api_key');
}

// 글로벌 함수 노출 (HTML onclick 이벤트용)
window.sendMessage = sendMessage;
window.resetChat = resetChat;
window.showApiKeyModal = showApiKeyModal;
window.closeModal = closeModal;
window.saveApiKey = saveApiKey;
