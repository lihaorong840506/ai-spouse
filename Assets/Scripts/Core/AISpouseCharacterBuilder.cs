using UnityEngine;
using AISpouse.UI;

namespace AISpouse.Core
{
    /// <summary>
    /// AI 배우자용 캐릭터 빌더
    /// 다정하고 따뜻한 AI 배우자의 컨셉에 맞는 캐릭터 생성
    /// </summary>
    public class AISpouseCharacterBuilder : MonoBehaviour
    {
        [Header("캐릭터 컨셉")]
        [SerializeField]
        private CharacterConcept _concept = CharacterConcept.GentleWife;

        [Header("캐릭터 색상")]
        [SerializeField]
        private Color _skinColor = new Color(1f, 0.9f, 0.85f);

        [SerializeField]
        private Color _eyeColor = new Color(0.3f, 0.3f, 0.35f);

        [SerializeField]
        private Color _hairColor = new Color(0.65f, 0.45f, 0.3f);

        [SerializeField]
        private Color _dressblazerColor = new Color(1f, 0.7f, 0.8f);

        [SerializeField]
        private Color _pantsColor = new Color(0.3f, 0.35f, 0.45f);

        [Header("캐릭터 크기")]
        [SerializeField]
        private float _characterHeight = 2f;

        [SerializeField]
        private float _characterWidth = 0.7f;

        [Header("스폰 설정")]
        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private bool _spawnOnStart = true;

        private GameObject _character;
        private GameObject _head;
        private GameObject _body;
        private GameObject _leftArm;
        private GameObject _rightArm;

        private void Start()
        {
            if (_spawnOnStart)
            {
                BuildCharacter();
            }
        }

        /// <summary>
        /// AI 배우자 캐릭터 생성
        /// </summary>
        public void BuildCharacter()
        {
            Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;

            // 캐릭터 루트 생성
            _character = new GameObject("AISpouse_Character");
            _character.transform.position = position;

            // 캐릭터 컨셉에 따라 색상 설정
            ApplyConceptColors();

            // 신체 부품 생성
            CreateBody();
            CreateHead();
            CreateFace();
            CreateHair();
            CreateArms();
            CreateLegs();
            CreateAccessories();

            // 컴포넌트 추가
            _character.AddComponent<CharacterAnimationController>();

            var animator = _character.AddComponent<Animator>();
            animator.runtimeAnimatorController = null;

            // 히트박스/콜라이더 추가 (필요시)
            AddPhysicsComponents();

            Debug.Log("[AISpouseCharacterBuilder] AI 배우자 캐릭터 생성 완료");
        }

        private void ApplyConceptColors()
        {
            switch (_concept)
            {
                case CharacterConcept.GentleWife:
                    _dressblazerColor = new Color(1f, 0.7f, 0.8f); // 핑크
                    _pantsColor = new Color(0.3f, 0.35f, 0.45f); // 네이비
                    _hairColor = new Color(0.65f, 0.45f, 0.3f); // 브라운
                    break;
                case CharacterConcept.ModernHusband:
                    _dressblazerColor = new Color(0.2f, 0.3f, 0.4f); // 네이비 블루
                    _pantsColor = new Color(0.15f, 0.15f, 0.15f); // 검정
                    _hairColor = new Color(0.15f, 0.1f, 0.05f); // 어두운 브라운
                    break;
                case CharacterConcept.CuteSpouse:
                    _dressblazerColor = new Color(1f, 0.85f, 0.6f); // 피치
                    _pantsColor = new Color(0.6f, 0.7f, 0.9f); // 라이트 블루
                    _hairColor = new Color(0.8f, 0.6f, 0.4f); // 골드 브라운
                    break;
                case CharacterConcept.WarmSpouse:
                    _dressblazerColor = new Color(0.9f, 0.6f, 0.4f); // 오렌지
                    _pantsColor = new Color(0.7f, 0.7f, 0.7f); // 그레이
                    _hairColor = new Color(0.7f, 0.5f, 0.3f); // 브라운
                    break;
            }
        }

        private void CreateBody()
        {
            // 상체 (캡슐)
            GameObject upperBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            upperBody.transform.SetParent(_character.transform);
            upperBody.transform.localPosition = Vector3.up * (_characterHeight * 0.5f);
            upperBody.transform.localScale = new Vector3(_characterWidth, _characterHeight * 0.45f, _characterWidth * 0.8f);
            upperBody.transform.rotation = Quaternion.Euler(90, 0, 0);
            upperBody.name = "UpperBody";
            SetMaterial(upperBody.GetComponent<Renderer>(), _dressblazerColor);

            // 하체 (캡슐)
            GameObject lowerBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            lowerBody.transform.SetParent(_character.transform);
            lowerBody.transform.localPosition = Vector3.up * (_characterHeight * 0.2f);
            lowerBody.transform.localScale = new Vector3(_characterWidth * 0.9f, _characterHeight * 0.25f, _characterWidth * 0.85f);
            lowerBody.transform.rotation = Quaternion.Euler(90, 0, 0);
            lowerBody.name = "LowerBody";
            SetMaterial(lowerBody.GetComponent<Renderer>(), _pantsColor);

            _body = upperBody;
        }

        private void CreateHead()
        {
            // 머리 (스피어)
            _head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _head.transform.SetParent(_character.transform);
            _head.transform.localPosition = Vector3.up * (_characterHeight * 0.8f);
            float headSize = _characterWidth * 1.3f;
            _head.transform.localScale = new Vector3(headSize, headSize * 1.1f, headSize);
            _head.name = "Head";
            SetMaterial(_head.GetComponent<Renderer>(), _skinColor);

            // 목 (캡슐)
            GameObject neck = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            neck.transform.SetParent(_character.transform);
            neck.transform.localPosition = Vector3.up * (_characterHeight * 0.7f);
            neck.transform.localScale = new Vector3(_characterWidth * 0.4f, _characterHeight * 0.08f, _characterWidth * 0.35f);
            neck.transform.rotation = Quaternion.Euler(90, 0, 0);
            neck.name = "Neck";
            SetMaterial(neck.GetComponent<Renderer>(), _skinColor);
        }

        private void CreateFace()
        {
            float headSize = _characterWidth * 1.3f;
            float eyeSize = _characterWidth * 0.12f;
            float eyeOffset = _characterWidth * 0.3f;
            float headHeight = _characterHeight * 0.8f;

            // 왼쪽 눈 (타원형을 위한 스케일드 스피어)
            GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftEye.transform.SetParent(_character.transform);
            leftEye.transform.localPosition = new Vector3(-eyeOffset, headHeight + eyeSize * 0.5f, _characterWidth * 0.45f);
            leftEye.transform.localScale = new Vector3(eyeSize, eyeSize * 0.6f, eyeSize * 0.5f);
            leftEye.name = "LeftEye";
            SetMaterial(leftEye.GetComponent<Renderer>(), Color.white);

            // 왼쪽 동공
            GameObject leftPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftPupil.transform.SetParent(leftEye.transform);
            leftPupil.transform.localPosition = new Vector3(0, 0, 0.03f);
            leftPupil.transform.localScale = new Vector3(0.6f, 0.8f, 0.5f);
            leftPupil.name = "LeftPupil";
            SetMaterial(leftPupil.GetComponent<Renderer>(), _eyeColor);

            // 오른쪽 눈
            GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightEye.transform.SetParent(_character.transform);
            rightEye.transform.localPosition = new Vector3(eyeOffset, headHeight + eyeSize * 0.5f, _characterWidth * 0.45f);
            rightEye.transform.localScale = new Vector3(eyeSize, eyeSize * 0.6f, eyeSize * 0.5f);
            rightEye.name = "RightEye";
            SetMaterial(rightEye.GetComponent<Renderer>(), Color.white);

            // 오른쪽 동공
            GameObject rightPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPupil.transform.SetParent(rightEye.transform);
            rightPupil.transform.localPosition = new Vector3(0, 0, 0.03f);
            rightPupil.transform.localScale = new Vector3(0.6f, 0.8f, 0.5f);
            rightPupil.name = "RightPupil";
            SetMaterial(rightPupil.GetComponent<Renderer>(), _eyeColor);

            // 눈썹
            CreateEyebrow(-eyeOffset, headHeight + eyeSize, _characterWidth * 0.45f, false);
            CreateEyebrow(eyeOffset, headHeight + eyeSize, _characterWidth * 0.45f, true);

            // 코
            GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            nose.transform.SetParent(_character.transform);
            nose.transform.localPosition = new Vector3(0, headHeight, _characterWidth * 0.5f);
            nose.transform.localScale = new Vector3(_characterWidth * 0.1f, _characterWidth * 0.15f, _characterWidth * 0.1f);
            nose.transform.rotation = Quaternion.Euler(0, 0, 90);
            nose.name = "Nose";
            SetMaterial(nose.GetComponent<Renderer>(), _skinColor);

            // 입 (타원형)
            GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            mouth.transform.SetParent(_character.transform);
            mouth.transform.localPosition = new Vector3(0, headHeight - _characterWidth * 0.2f, _characterWidth * 0.45f);
            mouth.transform.localScale = new Vector3(_characterWidth * 0.25f, _characterWidth * 0.08f, _characterWidth * 0.05f);
            mouth.name = "Mouth";
            SetMaterial(mouth.GetComponent<Renderer>(), new Color(0.95f, 0.7f, 0.7f));

            // 볼 (좌우)
            GameObject leftCheek = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftCheek.transform.SetParent(_character.transform);
            leftCheek.transform.localPosition = new Vector3(-_characterWidth * 0.5f, headHeight - _characterWidth * 0.1f, _characterWidth * 0.4f);
            leftCheek.transform.localScale = new Vector3(_characterWidth * 0.25f, _characterWidth * 0.15f, _characterWidth * 0.15f);
            leftCheek.name = "LeftCheek";
            SetMaterial(leftCheek.GetComponent<Renderer>(), new Color(1f, 0.85f, 0.85f));

            GameObject rightCheek = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightCheek.transform.SetParent(_character.transform);
            rightCheek.transform.localPosition = new Vector3(_characterWidth * 0.5f, headHeight - _characterWidth * 0.1f, _characterWidth * 0.4f);
            rightCheek.transform.localScale = new Vector3(_characterWidth * 0.25f, _characterWidth * 0.15f, _characterWidth * 0.15f);
            rightCheek.name = "RightCheek";
            SetMaterial(rightCheek.GetComponent<Renderer>(), new Color(1f, 0.85f, 0.85f));

            // 귀 (좌)
            GameObject leftEar = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftEar.transform.SetParent(_character.transform);
            leftEar.transform.localPosition = new Vector3(-_characterWidth * 0.65f, headHeight, 0);
            leftEar.transform.localScale = new Vector3(_characterWidth * 0.15f, _characterWidth * 0.3f, _characterWidth * 0.1f);
            leftEar.name = "LeftEar";
            SetMaterial(leftEar.GetComponent<Renderer>(), _skinColor);

            GameObject rightEar = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightEar.transform.SetParent(_character.transform);
            rightEar.transform.localPosition = new Vector3(_characterWidth * 0.65f, headHeight, 0);
            rightEar.transform.localScale = new Vector3(_characterWidth * 0.15f, _characterWidth * 0.3f, _characterWidth * 0.1f);
            rightEar.name = "RightEar";
            SetMaterial(rightEar.GetComponent<Renderer>(), _skinColor);
        }

        private void CreateEyebrow(float xPos, float yPos, float zPos, bool isRight)
        {
            GameObject eyebrow = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            eyebrow.transform.SetParent(_character.transform);
            eyebrow.transform.localPosition = new Vector3(xPos, yPos + _characterWidth * 0.15f, zPos);
            eyebrow.transform.localScale = new Vector3(_characterWidth * 0.15f, _characterWidth * 0.05f, _characterWidth * 0.05f);
            eyebrow.transform.rotation = Quaternion.Euler(0, 0, isRight ? -10 : 10);
            eyebrow.name = isRight ? "RightEyebrow" : "LeftEyebrow";
            SetMaterial(eyebrow.GetComponent<Renderer>(), _hairColor);
        }

        private void CreateHair()
        {
            float headSize = _characterWidth * 1.3f;

            // 머리카락 (상단)
            GameObject topHair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            topHair.transform.SetParent(_character.transform);
            topHair.transform.localPosition = new Vector3(0, _characterHeight * 0.9f, 0);
            topHair.transform.localScale = new Vector3(headSize * 1.1f, headSize * 0.4f, headSize * 1.05f);
            topHair.name = "TopHair";
            SetMaterial(topHair.GetComponent<Renderer>(), _hairColor);

            // 머리카락 (후방)
            GameObject backHair = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            backHair.transform.SetParent(_character.transform);
            backHair.transform.localPosition = new Vector3(0, _characterHeight * 0.85f, -_characterWidth * 0.4f);
            backHair.transform.localScale = new Vector3(headSize * 0.9f, headSize * 0.5f, headSize * 0.4f);
            backHair.transform.rotation = Quaternion.Euler(90, 0, 0);
            backHair.name = "BackHair";
            SetMaterial(backHair.GetComponent<Renderer>(), _hairColor);

            // 앞머리 (뱅)
            GameObject bang = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bang.transform.SetParent(_character.transform);
            bang.transform.localPosition = new Vector3(0, _characterHeight * 0.88f, _characterWidth * 0.5f);
            bang.transform.localScale = new Vector3(headSize * 0.95f, headSize * 0.15f, headSize * 0.15f);
            bang.name = "Bang";
            SetMaterial(bang.GetComponent<Renderer>(), _hairColor);
        }

        private void CreateArms()
        {
            float armWidth = _characterWidth * 0.2f;
            float armLength = _characterHeight * 0.35f;
            float armHeight = _characterHeight * 0.5f;
            float armOffset = _characterWidth * 0.6f;

            // 왼쪽 팔 (상체)
            _leftArm = CreateArmSegment(armOffset, armHeight, armWidth, armLength, false, _dressblazerColor, "LeftArm");

            // 왼쪽 팔 (하체)
            CreateArmSegment(armOffset, armHeight - armLength * 0.5f, armWidth, armLength * 0.8f, false, _dressblazerColor, "LeftForearm");

            // 오른쪽 팔 (상체)
            _rightArm = CreateArmSegment(-armOffset, armHeight, armWidth, armLength, true, _dressblazerColor, "RightArm");

            // 오른쪽 팔 (하체)
            CreateArmSegment(-armOffset, armHeight - armLength * 0.5f, armWidth, armLength * 0.8f, true, _dressblazerColor, "RightForearm");

            // 손 (좌)
            CreateHand(armOffset, armHeight - armLength * 0.9f, false, "LeftHand");
            CreateHand(-armOffset, armHeight - armLength * 0.9f, true, "RightHand");
        }

        private GameObject CreateArmSegment(float xPos, float yPos, float width, float length, bool isRight, Color color, string name)
        {
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            arm.transform.SetParent(_character.transform);
            arm.transform.localPosition = new Vector3(xPos, yPos, 0);
            arm.transform.localScale = new Vector3(width, length, width);
            arm.transform.rotation = Quaternion.Euler(0, 0, isRight ? -15 : 15);
            arm.name = name;
            SetMaterial(arm.GetComponent<Renderer>(), color);
            return arm;
        }

        private void CreateHand(float xPos, float yPos, bool isRight, string name)
        {
            GameObject hand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hand.transform.SetParent(_character.transform);
            hand.transform.localPosition = new Vector3(xPos, yPos, 0);
            hand.transform.localScale = new Vector3(_characterWidth * 0.15f, _characterWidth * 0.12f, _characterWidth * 0.12f);
            hand.name = name;
            SetMaterial(hand.GetComponent<Renderer>(), _skinColor);
        }

        private void CreateLegs()
        {
            float legWidth = _characterWidth * 0.25f;
            float legLength = _characterHeight * 0.25f;
            float legOffset = _characterWidth * 0.25f;
            float legHeight = _characterHeight * 0.15f;

            // 왼쪽 다리 (상체)
            CreateLegSegment(-legOffset, legHeight, legWidth, legLength, _pantsColor, "LeftThigh");

            // 왼쪽 다리 (하체)
            CreateLegSegment(-legOffset, legHeight - legLength * 0.5f, legWidth, legLength, _pantsColor, "LeftShin");

            // 오른쪽 다리 (상체)
            CreateLegSegment(legOffset, legHeight, legWidth, legLength, _pantsColor, "RightThigh");

            // 오른쪽 다리 (하체)
            CreateLegSegment(legOffset, legHeight - legLength * 0.5f, legWidth, legLength, _pantsColor, "RightShin");

            // 발 (좌)
            CreateFoot(-legOffset, 0.05f, "LeftFoot");
            CreateFoot(legOffset, 0.05f, "RightFoot");
        }

        private void CreateLegSegment(float xPos, float yPos, float width, float length, Color color, string name)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            leg.transform.SetParent(_character.transform);
            leg.transform.localPosition = new Vector3(xPos, yPos, 0);
            leg.transform.localScale = new Vector3(width, length, width);
            leg.transform.rotation = Quaternion.Euler(90, 0, 0);
            leg.name = name;
            SetMaterial(leg.GetComponent<Renderer>(), color);
        }

        private void CreateFoot(float xPos, float yPos, string name)
        {
            GameObject foot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foot.transform.SetParent(_character.transform);
            foot.transform.localPosition = new Vector3(xPos, yPos, _characterWidth * 0.2f);
            foot.transform.localScale = new Vector3(_characterWidth * 0.2f, _characterWidth * 0.05f, _characterWidth * 0.35f);
            foot.name = name;
            SetMaterial(foot.GetComponent<Renderer>(), new Color(0.2f, 0.2f, 0.2f)); // 검정색 신발
        }

        private void CreateAccessories()
        {
            // 넥타이/목걸이 (선택적)
            GameObject necktie = GameObject.CreatePrimitive(PrimitiveType.Cube);
            necktie.transform.SetParent(_character.transform);
            necktie.transform.localPosition = new Vector3(0, _characterHeight * 0.55f, _characterWidth * 0.3f);
            necktie.transform.localScale = new Vector3(_characterWidth * 0.1f, _characterHeight * 0.25f, _characterWidth * 0.05f);
            necktie.name = "Necktie";
            SetMaterial(necktie.GetComponent<Renderer>(), new Color(0.8f, 0.3f, 0.3f)); // 빨간색 넥타이

            // 헤어 악세서리 (리본)
            GameObject ribbon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ribbon.transform.SetParent(_character.transform);
            ribbon.transform.localPosition = new Vector3(0, _characterHeight * 0.92f, _characterWidth * 0.55f);
            ribbon.transform.localScale = new Vector3(_characterWidth * 0.2f, _characterWidth * 0.05f, _characterWidth * 0.05f);
            ribbon.name = "Ribbon";
            SetMaterial(ribbon.GetComponent<Renderer>(), new Color(1f, 0.6f, 0.7f));
        }

        private void AddPhysicsComponents()
        {
            Rigidbody rb = _character.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            CapsuleCollider collider = _character.AddComponent<CapsuleCollider>();
            collider.height = _characterHeight * 0.8f;
            collider.radius = _characterWidth * 0.5f;
            collider.center = Vector3.up * (_characterHeight * 0.4f);
        }

        private void SetMaterial(Renderer renderer, Color color)
        {
            renderer.material.color = color;
            renderer.material.SetFloat("_Smoothness", 0.3f);
        }

        /// <summary>
        /// 캐릭터 반환
        /// </summary>
        public GameObject GetCharacter()
        {
            return _character;
        }

        /// <summary>
        /// 머리 반환 (애니메이션용)
        /// </summary>
        public GameObject GetHead()
        {
            return _head;
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
                _head = null;
                _body = null;
                _leftArm = null;
                _rightArm = null;
            }
        }

        /// <summary>
        /// 캐릭터 다시 생성
        /// </summary>
        public void RespawnCharacter()
        {
            DestroyCharacter();
            BuildCharacter();
        }

        private void OnDrawGizmos()
        {
            // 스폰 위치 표시
            Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, 0.5f);
            Gizmos.DrawLine(position, position + Vector3.up * _characterHeight);
        }
    }

    /// <summary>
    /// 캐릭터 컨셉
    /// </summary>
    public enum CharacterConcept
    {
        GentleWife,      // 다정한 아내 컨셉
        ModernHusband,   // 모던한 남편 컨셉
        CuteSpouse,      // 귀여운 배우자 컨셉
        WarmSpouse       // 따뜻한 배우자 컨셉
    }
}
