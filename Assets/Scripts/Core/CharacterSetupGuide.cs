using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AISpouse.Core
{
    /// <summary>
    /// 캐릭터 설정 가이드 - 인스펙터에서 캐릭터 설정 방법 안내
    /// </summary>
    public class CharacterSetupGuide : MonoBehaviour
    {
        [Header("캐릭터 소스")]
        [Tooltip("캐릭터 소스")]
        [SerializeField]
        private CharacterSourceType _sourceType = CharacterSourceType.Mixamo;

        public CharacterSourceType SourceType => _sourceType;

        [Tooltip("Mixamo URL (소스가 Mixamo인 경우)")]
        [SerializeField]
        private string _mixamoUrl = "https://www.mixamo.com";

        [Tooltip("프리팹 경로 (직접 로드 시)")]
        [SerializeField]
        private string _prefabPath = "Assets/Models/CharacterPrefab.prefab";

        [Header("애니메이션 설정")]
        [Tooltip("애니메이터 컨트롤러 에셋")]
        [SerializeField]
        private RuntimeAnimatorController _animatorController;

        [Tooltip("기본 애니메이션 클립")]
        [SerializeField]
        private AnimationClip _idleAnimation;

        private void OnValidate()
        {
            // 캐릭터 설정이 올바른지 검증
        }
    }

    public enum CharacterSourceType
    {
        Mixamo,           // Mixamo에서 다운로드
        AssetStore,       // Unity Asset Store
        CustomPrefab,     // 직접 제작한 프리팹
        Live2D            // Live2D
    }
}

#if UNITY_EDITOR
namespace AISpouse.Core.Editor
{
    [CustomEditor(typeof(CharacterSetupGuide))]
    public class CharacterSetupGuideEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SerializedProperty sourceTypeProp = serializedObject.FindProperty("_sourceType");
            CharacterSourceType sourceType = (CharacterSourceType)sourceTypeProp.enumValueIndex;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("캐릭터 설정 가이드", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawSourceGuide(sourceType);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("캐릭터 설정 완료 후 CharacterSpawner 컴포넌트에 프리팹을 할당하세요.", MessageType.Info);
        }

        private void DrawSourceGuide(CharacterSourceType source)
        {
            switch (source)
            {
                case CharacterSourceType.Mixamo:
                    DrawMixamoGuide();
                    break;
                case CharacterSourceType.AssetStore:
                    DrawAssetStoreGuide();
                    break;
                case CharacterSourceType.CustomPrefab:
                    DrawCustomPrefabGuide();
                    break;
                case CharacterSourceType.Live2D:
                    DrawLive2DGuide();
                    break;
            }
        }

        private void DrawMixamoGuide()
        {
            EditorGUILayout.LabelField("Mixamo 캐릭터 설정", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. Mixamo(https://www.mixamo.com)에서 캐릭터 선택\n" +
                "2. 원하는 애니메이션 선택 (Idle, Happy, Sad 등)\n" +
                "3. 다운로드 후 Unity로 임포트\n" +
                "4. 프리팹으로 변환 후 CharacterSpawner에 할당",
                MessageType.Info
            );
        }

        private void DrawAssetStoreGuide()
        {
            EditorGUILayout.LabelField("Asset Store 캐릭터 설정", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. Unity Asset Store에서 캐릭터 다운로드\n" +
                "2. 추천: UMA 2, Toon Character Maker\n" +
                "3. 프리팹으로 변환 후 CharacterSpawner에 할당",
                MessageType.Info
            );
        }

        private void DrawCustomPrefabGuide()
        {
            EditorGUILayout.LabelField("커스텀 프리팹 설정", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. Blender 등 3D 툴로 모델 제작\n" +
                "2. Unity로 임포트 후 Rigging 설정\n" +
                "3. 애니메이션 컨트롤러 생성\n" +
                "4. 프리팹으로 저장 후 CharacterSpawner에 할당",
                MessageType.Info
            );
        }

        private void DrawLive2DGuide()
        {
            EditorGUILayout.LabelField("Live2D 캐릭터 설정", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. Live2D Cubism Editor로 모델 제작\n" +
                "2. Unity Live2D SDK 임포트\n" +
                "3. Live2D Runtime 컴포넌트 추가\n" +
                "4. Live2DController를 사용하여 표정 제어",
                MessageType.Info
            );
        }
    }
}
#endif
