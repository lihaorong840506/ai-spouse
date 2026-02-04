# AI 배우자 (AI Spouse) - 프로토타입

## 1단계: 프로토타입 구현 완료 ✅

Unity와 OpenAI를 연결하여 텍스트 대화가 가능한 기본 챗봇 시스템을 구현했습니다.

## 📁 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameConfig.cs         # 게임 설정 및 API 키 관리
│   │   └── ConversationManager.cs # 대화 흐름 관리
│   ├── AI/
│   │   └── OpenAIClient.cs       # OpenAI API 통신
│   └── UI/
│       └── ChatUI.cs             # 채팅 UI 관리
└── Prompts/Persona/
    └── DefaultPersona.txt        # AI 페르소나 설정
```

## 🚀 빠른 시작

### 1. Unity 프로젝트 설정
1. Unity Hub에서 새 2D 또는 3D 프로젝트 생성
2. 이 프로젝트의 `Assets/Scripts`와 `Assets/Prompts` 폴더를 Unity 프로젝트로 복사
3. 필요한 패키지 설치:
   - TextMeshPro (Unity 기본 패키지)
   - JSON .NET (NuGet 또는 Unity Asset Store)

### 2. API 키 설정
1. Unity에서 `Assets/Create/AI Spouse/Game Config` 메뉴로 GameConfig 에셋 생성
2. 생성된 에셋을 선택하고 Inspector에서 OpenAI API 키 입력
3. ⚠️ **중요**: API 키는 `.env` 파일이나 환경 변수에서 로드하는 것을 권장하며, 절대로 Git에 커밋하지 마세요!

### 3. 씬 구성
1. 새 씬 생성
2. 빈 게임 오브젝트 생성 후 `ConversationManager` 스크립트 추가
3. 같은 오브젝트에 `OpenAIClient` 스크립트 추가
4. GameConfig 에셋을 OpenAIClient의 `Game Config` 필드에 연결
5. UI Canvas 생성:
   - TMP_InputField (입력창)
   - Button (전송 버튼)
   - Scroll View (메시지 표시 영역)
   - 빈 게임 오브젝트(ChatUI)에 `ChatUI` 스크립트 추가
   - UI 요소들을 ChatUI 컴포넌트의 필드에 연결

### 4. 테스트
1. Play 버튼 클릭
2. 입력창에 메시지 입력
3. 전송 버튼 클릭 또는 Enter 키 누름
4. AI 응답 확인!

## ⚙️ 설정 옵션

### GameConfig
- **OpenAI API Key**: OpenAI에서 발급받은 API 키
- **Model Name**: 사용할 GPT 모델 (기본: gpt-3.5-turbo)
- **Max Tokens**: 최대 응답 토큰 수
- **Temperature**: 창의성 조절 (0.0~1.0)
- **Max Conversation History**: 기억할 최근 대화 수

### 페르소나 커스터마이징
`Assets/Prompts/Persona/DefaultPersona.txt` 파일을 수정하여 AI의 성격을 변경할 수 있습니다.

또는 ConversationManager 컴포넌트에서:
- **Persona Prompt Asset**: 다른 페르소나 파일을 TextAsset으로 연결
- **Custom System Prompt**: 직접 시스템 프롬프트 입력

## 🛠️ 구현된 기능

✅ OpenAI API 통신  
✅ 대화 히스토리 관리 (최근 N개 대화 기억)  
✅ 시스템 프롬프트 (페르소나) 설정  
✅ 기본 채팅 UI  
✅ 에러 처리 및 디버깅 로그  

## 📋 다음 단계

### 2단계: 시각화 및 청각화
- [ ] Live2D/3D 캐릭터 모델 추가
- [ ] 감정 태깅 시스템 ([행복], [슬픔])
- [ ] TTS 연동 (ElevenLabs 또는 OpenAI TTS)

### 3단계: 단기 기억
- [ ] 엔티티 추출 (사용자 취향, 정보 기록)
- [ ] 로컬 저장 (PlayerPrefs 또는 JSON)

## 📚 참고 문서

- [설계서.md](설계서.md) - 시스템 설계 문서
- [프로토타입1.md](프로토타입1.md) - 5단계 개발 로드맵

## ⚠️ 주의사항

- API 키는 노출되지 않도록 주의하세요
- `.env` 파일 또는 환경 변수 사용을 권장합니다
- API 호출 비용이 발생하므로 테스트 시 주의하세요

---

*Made with 💕 for AI Spouse Project*
