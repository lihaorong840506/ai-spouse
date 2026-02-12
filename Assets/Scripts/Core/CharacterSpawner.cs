using UnityEngine;
using AISpouse.UI;

namespace AISpouse.Core
{
    /// <summary>
    /// 캐릭터 스포너 - 외부 에셋(Mixamo, Asset Store 등)에서 로드한 캐릭터를 생성
    /// </summary>
    public class CharacterSpawner : MonoBehaviour
    {
        [Header("캐릭터 프리팹")]
        [Tooltip("사용할 3D 캐릭터 프리팹")]
        [SerializeField]
        private GameObject _characterPrefab;

        [Header("스폰 설정")]
        [Tooltip("캐릭터가 생성될 위치")]
        [SerializeField]
        private Transform _spawnPoint;

        [Tooltip("생성 시 캐릭터 스케일")]
        [SerializeField]
        private Vector3 _characterScale = Vector3.one;

        [Tooltip("생성 시 캐릭터 회전")]
        [SerializeField]
        private Vector3 _characterRotation = Vector3.zero;

        [Header("카메라 설정")]
        [Tooltip("캐릭터를 바라볼 카메라")]
        [SerializeField]
        private Camera _targetCamera;

        [Tooltip("캐릭터와의 거리")]
        [SerializeField]
        private float _cameraDistance = 3f;

        private GameObject _spawnedCharacter;
        private CharacterAnimationController _animationController;

        private void Start()
        {
            SpawnCharacter();
        }

        /// <summary>
        /// 캐릭터 생성
        /// </summary>
        private void SpawnCharacter()
        {
            if (_characterPrefab == null)
            {
                Debug.LogWarning("[CharacterSpawner] 캐릭터 프리팹이 설정되지 않았습니다.");
                return;
            }

            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion spawnRotation = Quaternion.Euler(_characterRotation);

            _spawnedCharacter = Instantiate(_characterPrefab, spawnPosition, spawnRotation);
            _spawnedCharacter.name = "AI_Spouse_Character";
            _spawnedCharacter.transform.localScale = _characterScale;

            // 애니메이션 컨트롤러 연결
            _animationController = _spawnedCharacter.GetComponentInChildren<CharacterAnimationController>();

            if (_animationController == null)
            {
                _animationController = _spawnedCharacter.AddComponent<CharacterAnimationController>();
                Debug.Log("[CharacterSpawner] CharacterAnimationController 추가됨");
            }
            else
            {
                Debug.Log("[CharacterSpawner] 기존 CharacterAnimationController 발견");
            }

            // 카메라 설정
            SetupCamera();

            Debug.Log("[CharacterSpawner] 캐릭터 생성 완료");
        }

        /// <summary>
        /// 카메라 설정
        /// </summary>
        private void SetupCamera()
        {
            if (_targetCamera == null)
            {
                _targetCamera = Camera.main;
            }

            if (_targetCamera != null && _spawnedCharacter != null)
            {
                // 카메라를 캐릭터 바라보도록 설정
                Vector3 cameraPosition = _spawnedCharacter.transform.position;
                cameraPosition.z -= _cameraDistance;
                _targetCamera.transform.position = cameraPosition;
                _targetCamera.transform.LookAt(_spawnedCharacter.transform);

                Debug.Log("[CharacterSpawner] 카메라 설정 완료");
            }
        }

        /// <summary>
        /// 생성된 캐릭터 반환
        /// </summary>
        public GameObject GetSpawnedCharacter()
        {
            return _spawnedCharacter;
        }

        /// <summary>
        /// 애니메이션 컨트롤러 반환
        /// </summary>
        public CharacterAnimationController GetAnimationController()
        {
            return _animationController;
        }

        /// <summary>
        /// 캐릭터 파괴
        /// </summary>
        public void DespawnCharacter()
        {
            if (_spawnedCharacter != null)
            {
                Destroy(_spawnedCharacter);
                _spawnedCharacter = null;
                _animationController = null;
                Debug.Log("[CharacterSpawner] 캐릭터 삭제됨");
            }
        }

        /// <summary>
        /// 캐릭터 다시 생성
        /// </summary>
        public void RespawnCharacter()
        {
            DespawnCharacter();
            SpawnCharacter();
        }

        private void OnDrawGizmos()
        {
            // 스폰 위치 표시
            Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, 0.5f);
            Gizmos.DrawLine(position, position + Vector3.up * 1.5f);

            // 카메라 위치 표시
            if (_spawnedCharacter != null)
            {
                Vector3 cameraPos = _spawnedCharacter.transform.position;
                cameraPos.z -= _cameraDistance;

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(cameraPos, 0.3f);
                Gizmos.DrawLine(cameraPos, _spawnedCharacter.transform.position);
            }
        }
    }
}
