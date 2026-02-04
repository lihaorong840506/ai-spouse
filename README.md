# AI 배우자 (AI Spouse) - 프로토타입

## 1단계: 프로토타입 구현 완료 ✅

Unity와 Azure OpenAI를 연결하여 텍스트 대화가 가능한 기본 챗봇 시스템을 구현했습니다.

## 📁 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── AISpouse.asmdef         # Assembly Definition (컴파일 설정)
│   ├── Core/
│   │   ├── GameConfig.cs              # 게임 설정 및 API 키 관리
│   │   └── ConversationManager.cs     # 대화 흐름 관리
│   ├── AI/
│   │   └── AzureOpenAIClient.cs       # Azure OpenAI API 통신
│   └── UI/
│       └── ChatUI.cs                  # 채팅 UI 관리
└── Prompts/Persona/
    └── DefaultPersona.txt             # AI 페르소나 설정
```

## 🚀 빠른 시작

### 1. Unity 프로젝트 설정
1. Unity Hub에서 새 2D 또는 3D 프로젝트 생성 (Unity 2022.3 LTS 이상 권장)
2. 이 프로젝트의 `Assets/Scripts`와 `Assets/Prompts` 폴터를 Unity 프로젝트로 복사
3. TextMeshPro 패키지는 Unity 기본 포함 (추가 설치 불필요)

**참고**: Newtonsoft.Json 패키지가 **필요 없습니다**. Unity 내장 JsonUtility를 사용합니다.

### 2. API 키 설정
1. Unity에서 `Assets/Create/AI Spouse/Game Config` 메뉴로 GameConfig 에셋 생성
2. 생성된 에셋을 선택하고 Inspector에서 Azure OpenAI 설정 입력:
   - **Azure Endpoint**: `https://luoji-ai-azure.cognitiveservices.azure.com`
   - **Azure API Key**: (Azure 포털에서 발급받은 키)
   - **API Version**: `2024-12-01-preview`
   - **Deployment Name**: `gpt-4o`
3. ⚠️ **중요**: API 키는 절대로 Git에 커밋하지 마세요!

### 3. 씬 구성
1. 새 씬 생성
2. `GameObject > UI > Canvas` 생성
3. `GameObject > UI > Input Field - TextMeshPro` 생성 (이름: "InputField")
4. `GameObject > UI > Button - TextMeshPro` 생성 (이름: "SendButton")
5. `GameObject > UI > Scroll View` 생성 (이름: "ChatScrollView")
6. 빈 게임 오브젝트 생성 (이름: "AI_Spouse_Manager") 후 다음 컴포넌트 추가:
   - `AzureOpenAIClient` 스크립트
   - `ConversationManager` 스크립트
   - `ChatUI` 스크립트
7. 컴포넌트들 연결:
   - AzureOpenAIClient의 Game Config에 GameConfig 에셋 연결
   - ChatUI의 UI 요소들에 Scene의 InputField, SendButton 등 연결

**자세한 설정 방법**: [UNITY_SETUP_GUIDE.md](UNITY_SETUP_GUIDE.md) 참고

### 4. 테스트
1. Play 버튼 클릭
2. 입력창에 메시지 입력 (예: "안녕하세요!")
3. 전송 버튼 클릭 또는 Enter 키 누름
4. AI 응답 확인!

## ⚙️ 설정 옵션

### GameConfig
- **Azure Endpoint**: Azure OpenAI 서비스 엔드포인트 URL
- **Azure API Key**: Azure 포털에서 발급받은 API 키
- **API Version**: Azure OpenAI API 버전
- **Deployment Name**: Azure에 배포된 모델 이름 (예: gpt-4o)
- **Max Tokens**: 최대 응답 토큰 수
- **Temperature**: 창의성 조절 (0.0~1.0)
- **Max Conversation History**: 기억할 최근 대화 수

### 페르소나 커스터마이징
`Assets/Prompts/Persona/DefaultPersona.txt` 파일을 수정하여 AI의 성격을 변경할 수 있습니다.

또는 ConversationManager 컴포넌트에서:
- **Persona Prompt Asset**: 다른 페르소나 파일을 TextAsset으로 연결
- **Custom System Prompt**: 직접 시스템 프롬프트 입력

## 🛠️ 구현된 기능

✅ Azure OpenAI API 통신 (gpt-4o)
✅ 대화 히스토리 관리 (최근 N개 대화 기억)
✅ 시스템 프롬프트 (페르소나) 설정
✅ 기본 채팅 UI (TextMeshPro)
✅ 에러 처리 및 디버깅 로그
✅ Unity 내장 JsonUtility 사용 (외부 패키지 불필요)

## 📋 다음 단계

### 2단계: 시각화 및 청각화
- [ ] Live2D/3D 캐릭터 모델 추가
- [ ] 감정 태깅 시스템 ([행복], [슬픔])
- [ ] TTS 연동 (Azure TTS 또는 ElevenLabs)

### 3단계: 단기 기억
- [ ] 엔티티 추출 (사용자 취향, 정보 기록)
- [ ] 로컬 저장 (PlayerPrefs 또는 JSON)

## 🌐 웹 데모

Unity 설치 없이 웹 브라우저에서 테스트핳 수 있는 버전도 제공됩니다.

**경로**: `web-demo/` 폴터

**실행 방법**:
```bash
cd web-demo
python -m venv venv
source venv/bin/activate
pip install -r requirements.txt
cd backend
python app.py
```

브라우저에서 `http://localhost:8080` 접속

## 📚 참고 문서

- [UNITY_SETUP_GUIDE.md](UNITY_SETUP_GUIDE.md) - Unity 상세 설정 가이드
- [설계서.md](설계서.md) - 시스템 설계 문서
- [프로토타입1.md](프로토타입1.md) - 5단계 개발 로드맵

## ⚠️ 주의사항

- API 키는 노출되지 않도록 주의하세요
- GameConfig 에셋의 API 키는 Git에 커밋되지 않도록 주의하세요
- API 호출 비용이 발생하므로 테스트 시 주의하세요

---

*Made with 💕 for AI Spouse Project*
