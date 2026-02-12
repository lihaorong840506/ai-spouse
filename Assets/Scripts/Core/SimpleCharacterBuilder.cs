using UnityEngine;
using AISpouse.UI;

namespace AISpouse.Core
{
    /// <summary>
    /// 간단한 기하학적 캐릭터 빌더 (테스트용 플레이스홀더)
    /// </summary>
    public class SimpleCharacterBuilder : MonoBehaviour
    {
        [Header("캐릭터 색상")]
        [SerializeField]
        private Color _bodyColor = new Color(1f, 0.8f, 0.9f); // 핑크색

        [SerializeField]
        private Color _eyeColor = new Color(0.2f, 0.2f, 0.2f);

        [SerializeField]
        private Color _hairColor = new Color(0.4f, 0.3f, 0.2f);

        [Header("캐릭터 크기")]
        [SerializeField]
        private float _characterHeight = 2f;

        [SerializeField]
        private float _characterWidth = 0.8f;

        [SerializeField]
        private float _characterDepth = 0.5f;

        [Header("스폰 설정")]
        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private bool _spawnOnStart = true;

        private GameObject _character;

        private void Start()
        {
            if (_spawnOnStart)
            {
                BuildCharacter();
            }
        }

        /// <summary>
        /// 간단한 캐릭터 생성
        /// </summary>
        public void BuildCharacter()
        {
            Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;

            // 캐릭터 루트 생성
            _character = new GameObject("SimpleCharacter");
            _character.transform.position = position;

            // 바디 생성
            CreateBody();

            // 머리 생성
            CreateHead();

            // 눈 생성
            CreateEyes();

            // 머리카락 생성
            CreateHair();

            // 팔 생성
            CreateArms();

            // 다리 생성
            CreateLegs();

            // 애니메이션 컨트롤러 추가
            _character.AddComponent<CharacterAnimationController>();
            _character.AddComponent<Animator>();

            Debug.Log("[SimpleCharacterBuilder] 간단한 캐릭터 생성 완료");
        }

        private void CreateBody()
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(_character.transform);
            body.transform.localPosition = Vector3.up * (_characterHeight * 0.35f);
            body.transform.localScale = new Vector3(_characterWidth, _characterHeight * 0.4f, _characterDepth);
            body.name = "Body";

            // 머티리얼 설정
            Renderer renderer = body.GetComponent<Renderer>();
            renderer.material.color = _bodyColor;

            // 캡슐 회전 조정
            body.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        private void CreateHead()
        {
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.SetParent(_character.transform);
            head.transform.localPosition = Vector3.up * (_characterHeight * 0.75f);
            float headSize = _characterWidth * 1.2f;
            head.transform.localScale = new Vector3(headSize, headSize, headSize);
            head.name = "Head";

            Renderer renderer = head.GetComponent<Renderer>();
            renderer.material.color = _bodyColor;
        }

        private void CreateEyes()
        {
            float eyeSize = _characterWidth * 0.15f;
            float eyeOffset = _characterWidth * 0.35f;
            float headHeight = _characterHeight * 0.75f;

            // 왼쪽 눈
            GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftEye.transform.SetParent(_character.transform);
            leftEye.transform.localPosition = new Vector3(-eyeOffset, headHeight + eyeSize * 0.5f, _characterDepth * 0.3f);
            leftEye.transform.localScale = new Vector3(eyeSize, eyeSize * 0.5f, eyeSize);
            leftEye.name = "LeftEye";
            leftEye.GetComponent<Renderer>().material.color = _eyeColor;

            // 오른쪽 눈
            GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightEye.transform.SetParent(_character.transform);
            rightEye.transform.localPosition = new Vector3(eyeOffset, headHeight + eyeSize * 0.5f, _characterDepth * 0.3f);
            rightEye.transform.localScale = new Vector3(eyeSize, eyeSize * 0.5f, eyeSize);
            rightEye.name = "RightEye";
            rightEye.GetComponent<Renderer>().material.color = _eyeColor;
        }

        private void CreateHair()
        {
            GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hair.transform.SetParent(_character.transform);
            float headSize = _characterWidth * 1.2f;
            hair.transform.localPosition = new Vector3(0, _characterHeight * 0.85f, 0);
            hair.transform.localScale = new Vector3(headSize * 1.1f, headSize * 0.3f, headSize);
            hair.name = "Hair";

            Renderer renderer = hair.GetComponent<Renderer>();
            renderer.material.color = _hairColor;
        }

        private void CreateArms()
        {
            float armWidth = _characterWidth * 0.2f;
            float armLength = _characterHeight * 0.3f;
            float armHeight = _characterHeight * 0.35f;
            float armOffset = _characterWidth * 0.6f;

            // 왼쪽 팔
            GameObject leftArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            leftArm.transform.SetParent(_character.transform);
            leftArm.transform.localPosition = new Vector3(-armOffset, armHeight, 0);
            leftArm.transform.localScale = new Vector3(armWidth, armLength, armWidth);
            leftArm.transform.rotation = Quaternion.Euler(0, 0, 20);
            leftArm.name = "LeftArm";
            leftArm.GetComponent<Renderer>().material.color = _bodyColor;

            // 오른쪽 팔
            GameObject rightArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rightArm.transform.SetParent(_character.transform);
            rightArm.transform.localPosition = new Vector3(armOffset, armHeight, 0);
            rightArm.transform.localScale = new Vector3(armWidth, armLength, armWidth);
            rightArm.transform.rotation = Quaternion.Euler(0, 0, -20);
            rightArm.name = "RightArm";
            rightArm.GetComponent<Renderer>().material.color = _bodyColor;
        }

        private void CreateLegs()
        {
            float legWidth = _characterWidth * 0.25f;
            float legLength = _characterHeight * 0.35f;
            float legOffset = _characterWidth * 0.25f;

            // 왼쪽 다리
            GameObject leftLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            leftLeg.transform.SetParent(_character.transform);
            leftLeg.transform.localPosition = new Vector3(-legOffset, legLength * 0.5f, 0);
            leftLeg.transform.localScale = new Vector3(legWidth, legLength, legWidth);
            leftLeg.transform.rotation = Quaternion.Euler(90, 0, 0);
            leftLeg.name = "LeftLeg";
            leftLeg.GetComponent<Renderer>().material.color = _bodyColor;

            // 오른쪽 다리
            GameObject rightLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rightLeg.transform.SetParent(_character.transform);
            rightLeg.transform.localPosition = new Vector3(legOffset, legLength * 0.5f, 0);
            rightLeg.transform.localScale = new Vector3(legWidth, legLength, legWidth);
            rightLeg.transform.rotation = Quaternion.Euler(90, 0, 0);
            rightLeg.name = "RightLeg";
            rightLeg.GetComponent<Renderer>().material.color = _bodyColor;
        }

        /// <summary>
        /// 캐릭터 반환
        /// </summary>
        public GameObject GetCharacter()
        {
            return _character;
        }

        /// <summary>
        /// 캐릭터 파괴
        /// </summary>
        public void DestroyCharacter()
        {
            if (_character != null)
            {
                Destroy(_character);
                _character = null;
            }
        }

        private void OnDrawGizmos()
        {
            // 스폰 위치 표시
            Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(position, 0.5f);
            Gizmos.DrawLine(position, position + Vector3.up * _characterHeight);
        }
    }
}
