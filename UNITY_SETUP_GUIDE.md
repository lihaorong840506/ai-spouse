# AI 배우자 (AI Spouse) - Unity 프로젝트 설정 가이드

## 🎮 Unity 프로젝트 설정 방법

### 1. 필수 요구사항
- **Unity 2022.3 LTS** 또는 **Unity 6 (6000.x)**
- **TextMeshPro** 패키지 (Unity 기본 포함)

### 2. 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── AISpouse.asmdef      # Assembly Definition
│   ├── Core/
│   │   ├── GameConfig.cs           # 게임 설정
│   │   └── ConversationManager.cs  # 대화 관리
│   ├── AI/
│   │   └── AzureOpenAIClient.cs    # Azure OpenAI API 클라이언트
│   └── UI/
│       └── ChatUI.cs               # 채팅 UI
└── Prompts/Persona/
    └── DefaultPersona.txt   # AI 페르소나 설정
```

### 3. 씬 설정 방법

#### Step 1: 새 씬 생성
1. Unity에서 `File > New Scene` 선택
2. `2D` 또는 `3D` 선택 (둘 다 가능)

#### Step 2: UI Canvas 생성
1. `GameObject > UI > Canvas` 생성
2. Canvas 설정:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler**:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1080 x 1920 (Portrait)

#### Step 3: 채팅 UI 구성

**Scroll View 생성:**
1. `GameObject > UI > Scroll View` 생성
2. 이름을 "ChatScrollView"로 변경
3. 크기 조정: Width: 1000, Height: 1500
4. **Content** 오브젝트에 **Vertical Layout Group** 컴포넌트 추가
   - Padding: 20
   - Spacing: 10
   - Child Alignment: Upper Center

**Input Field 생성:**
1. `GameObject > UI > Input Field - TextMeshPro` 생성
2. 이름을 "InputField"로 변경
3. 위치: 하단 중앙
4. 크기: Width: 800, Height: 100

**Send Button 생성:**
1. `GameObject > UI > Button - TextMeshPro` 생성
2. 이름을 "SendButton"로 변경
3. 텍스트를 "전송"으로 변경
4. 위치: InputField 옆

#### Step 4: 스크립트 추가

**GameConfig 에셋 생성:**
1. `Assets/Create/AI Spouse/Game Config` 메뉴 선택
2. "GameConfig" 이름으로 저장
3. Inspector에서 Azure 설정 입력:
   - **Azure Endpoint**: `https://luoji-ai-azure.cognitiveservices.azure.com`
   - **Azure API Key**: (실제 API 키 입력)
   - **API Version**: `2024-12-01-preview`
   - **Deployment Name**: `gpt-4o`

**메인 GameObject 설정:**
1. 빈 GameObject 생성 (이름: "AI_Spouse_Manager")
2. 다음 컴포넌트 추가:
   - **AzureOpenAIClient** 스크립트
   - **ConversationManager** 스크립트
   - **ChatUI** 스크립트

3. **AzureOpenAIClient** 컴포넌트:
   - Game Config 필드에 방금 생성한 GameConfig 에셋 연결

4. **ConversationManager** 컴포넌트:
   - Azure Open AI Client 필드에 같은 오브젝트의 AzureOpenAIClient 연결
   - (선택) Persona Prompt Asset에 DefaultPersona.txt 연결

5. **ChatUI** 컴포넌트:
   - Input Field: Scene의 InputField 연결
   - Send Button: Scene의 SendButton 연결
   - Scroll Rect: Scene의 ChatScrollView 연결
   - Message Container: ChatScrollView > Content 연결

### 4. API 키 설정

**GameConfig 에셋 설정:**
1. Project 창에서 GameConfig 에셋 선택
2. Inspector에서 설정:
   - **Azure Api Key**: (Azure 포털에서 발급받은 API 키 입력)
   - 나머지 설정은 기본값 유지

### 5. 실행 테스트

1. **Play 버튼** 클릭
2. Input Field에 메시지 입력 (예: "안녕하세요, 여보!")
3. **전송 버튼** 클릭 또는 **Enter** 키 누름
4. AI 응답 확인!

### 6. 흔한 오류 해결

**"GameConfig가 설정되지 않았습니다" 오류:**
- AzureOpenAIClient 컴포넌트의 Game Config 필드가 비어있음
- GameConfig 에셋을 생성하고 연결하세요

**"API 키 오류" 메시지:**
- GameConfig의 Azure Api Key가 비어있거나 잘못됨
- 올바른 API 키를 입력하세요

**"InputField를 찾을 수 없습니다" 오류:**
- Scene에 TMP_InputField가 있는 GameObject 이름이 "InputField"가 아님
- GameObject 이름을 "InputField"로 변경하거나 ChatUI 컴포넌트에 직접 연결하세요

**UI가 보이지 않음:**
- Canvas의 Render Mode 확인
- UI 요소들이 Canvas의 자식인지 확인

### 7. 빌드 설정 (모바일)

**Android 빌드:**
1. `File > Build Settings` > Android 선택
2. Player Settings:
   - Internet Access: Required
   - Package Name 설정
3. Build

**iOS 빌드:**
1. `File > Build Settings` > iOS 선택
2. Player Settings에서 Bundle Identifier 설정
3. Build

---

## 🔧 수정 사항 요약

이번 업데이트에서 변경된 내용:

1. **Azure OpenAI 통합**: 기존 OpenAI API에서 Azure OpenAI로 변경
2. **JsonUnity**: Newtonsoft.Json 대신 Unity 내장 JsonUtility 사용
3. **Assembly Definition**: AISpouse.asmdef 추가로 컴파일 문제 해결
4. **ChatUI 개선**: 프리팹 없이도 작동하도록 개선
5. **GameConfig 업데이트**: Azure 설정 필드 추가

---

**질문이 있으면 언제든지 물어보세요!** 💕
