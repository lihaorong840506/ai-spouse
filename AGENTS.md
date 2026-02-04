# AGENTS.md - AI 배우자 (AI Spouse) Game Project

## Project Context

AI companion game built in Unity with OpenAI integration. Features long-term memory system using RAG (Retrieval-Augmented Generation) to create personalized NPC interactions. Project follows 5-stage MVP roadmap (see 프로토타입1.md).

**Tech Stack**: Unity (C#), OpenAI API, Vector DB (Pinecone/Supabase), optional Python backend

## Communication Rules

**All replies MUST be in Korean (한국어).**

This includes:
- All documentation and comments
- UI text and prompts
- Variable names with Korean meaning (e.g., `_userName` not `_yongHuIReum`)
- Git commit messages
- Code review feedback
- All conversations with users

**Example**:
```csharp
// Good: Comment in Korean
// 사용자 입력을 처리하고 AI에게 전송합니다
public async Task ProcessUserInput(string userInput)

// Bad: Comment in English
// Process user input and send to AI
```

## Build Commands

Since this is a Unity project, standard Unity build commands apply:

```bash
# Unity CLI builds (when project is ready)
unity -quit -batchmode -buildWindows64Player ./Builds/Windows/AI_Spouse.exe
unity -quit -batchmode -buildAndroidPlayer ./Builds/Android/ai_spouse.apk
unity -quit -batchmode -buildiOSPlayer ./Builds/iOS
```

**Test Commands** (to be configured when tests are added):
```bash
# Unity Test Framework (when implemented)
unity -runTests -testPlatform editmode
unity -runTests -testPlatform playmode

# Single test run (example)
unity -runTests -testPlatform editmode -testFilter "MemorySystemTests"
```

## Code Style Guidelines

### C# (Unity)

**Naming Conventions**:
- Classes/Methods: PascalCase (`ConversationManager`, `ExtractUserInfo()`)
- Private fields: _camelCase (`_memoryDatabase`, `_openAIClient`)
- Public fields: camelCase (`userName`, `conversationHistory`)
- Constants: UPPER_SNAKE_CASE (`MAX_CONVERSATION_HISTORY`)
- Interfaces: IPascalCase (`IMemoryStorage`, `IAIProvider`)

**Formatting**:
- 4 spaces for indentation
- Opening braces on same line
- Max 120 characters per line
- Always use braces for single-line conditionals

**Type Safety**:
- NEVER use `dynamic` or implicit `var` for public APIs
- Use explicit types for clarity: `List<ConversationEntry> history = new List<ConversationEntry>();`
- Use `SerializeField` for Unity Inspector visibility instead of public fields

**Imports**:
```csharp
// System first
using System;
using System.Collections.Generic;

// Unity second
using UnityEngine;
using UnityEngine.UI;

// Third-party
using OpenAI;
using Newtonsoft.Json;

// Project last
using AISpouse.Memory;
using AISpouse.UI;
```

### Python (Backend/LLM Processing)

**Naming**:
- Functions/variables: snake_case (`extract_entities()`, `memory_vector`)
- Classes: PascalCase (`MemoryExtractor`, `VectorStore`)
- Constants: UPPER_SNAKE_CASE

**Formatting**:
- 4 spaces indentation
- Max 100 characters per line
- Use type hints: `def extract_info(text: str) -> dict[str, Any]:`

## Architecture Patterns

### Component Structure
```csharp
// AI Spouse Architecture
Assets/
├── Scripts/
│   ├── Core/           # GameManager, Config
│   ├── Memory/         # MemoryStorage, EntityExtractor
│   ├── AI/             # OpenAIClient, PromptBuilder
│   ├── UI/             # ChatUI, MemoryNotesUI
│   └── Character/      # AnimationController, TTSService
```

### Memory System Flow
1. `ConversationManager` captures user input
2. `EntityExtractor` (LLM) parses for entities → JSON
3. `MemoryStorage` saves to Vector DB + Structured DB
4. `ContextBuilder` retrieves relevant memories for next prompt

### Error Handling
```csharp
// Use Result<T> pattern for external calls
try {
    var response = await _openAIClient.SendMessageAsync(prompt);
    return Result<string>.Success(response);
} catch (OpenAIException ex) {
    Debug.LogError($"[AI] API Error: {ex.Message}");
    return Result<string>.Failure("AI temporarily unavailable");
}
```

## AI Integration Patterns

**Prompt Engineering**:
- System prompts define persona in `Prompts/Persona/` folder
- Use JSON mode for structured outputs (entity extraction)
- Tag emotional state: `[행복]`, `[슬픔]`, `[걱정]` for animation triggers

**Entity Extraction JSON Schema**:
```json
{
  "category": "food_preference|relationship|emotion|schedule",
  "item": "mint_chocolate",
  "value": "like|dislike|neutral",
  "confidence": 0.95,
  "context": "user mentioned during dinner conversation"
}
```

## Development Workflow

**5-Stage MVP** (see 프로토타입1.md):
1. Prototype: Basic chat (OpenAI + Unity UI)
2. Visualization: Live2D/3D model + TTS
3. Memory: Short-term context + entity extraction
4. Game Systems: Affinity, scheduler, notifications
5. Optimization: RAG, local LLM, monetization

**Commit Style**:
- `feat: Add memory entity extraction`
- `fix: Resolve TTS async timing issue`
- `docs: Update memory system design (설계서.md)`

**Before Submitting**:
- Test in Unity Editor Play mode
- Verify no `Debug.Log` spam in production code
- Check Korean text encoding (UTF-8)
- Ensure API keys are in `.env` or `Config/` (never committed)

## Key Files

- `설계서.md` - System design (Korean)
- `프로토타입1.md` - 5-stage roadmap (Korean)
- `Assets/Scripts/Core/GameConfig.cs` - Configuration (when created)
- `Assets/Scripts/Memory/` - Memory system implementation

## External Dependencies

- OpenAI API (GPT-4, GPT-3.5-turbo)
- Optional: ElevenLabs (TTS), Pinecone (Vector DB), Supabase (DB)
- Unity packages: TextMeshPro, UnityWebRequest, JSON .NET

---

*Generated for AI agentic coding assistants working on this repository.*
