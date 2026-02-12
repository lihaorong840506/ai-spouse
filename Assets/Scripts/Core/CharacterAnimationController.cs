using System.Collections.Generic;
using UnityEngine;
using AISpouse.Core;

namespace AISpouse.UI
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [Header("애니메이션 설정")]
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _crossFadeDuration = 0.3f;

        [Header("애니메이션 파라미터 이름")]
        [SerializeField]
        private string _emotionParamName = "Emotion";

        [SerializeField]
        private string _intensityParamName = "Intensity";

        [SerializeField]
        private string _isTalkingParamName = "IsTalking";

        [Header("애니메이션 클립 이름 매핑")]
        [SerializeField]
        private AnimationNameMapping _animationNames;

        private EmotionType _currentEmotion = EmotionType.Neutral;
        private bool _isTalking = false;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null)
                {
                    Debug.LogError("[CharacterAnimationController] Animator 컴포넌트를 찾을 수 없습니다!");
                }
            }
        }

        private void Start()
        {
            SetEmotion(EmotionType.Neutral, 50);
        }

        /// <summary>
        /// 감정에 맞는 애니메이션 재생
        /// </summary>
        public void PlayEmotionAnimation(EmotionType emotion, float intensity = 1.0f)
        {
            if (_animator == null)
            {
                Debug.LogWarning("[CharacterAnimationController] Animator가 없습니다.");
                return;
            }

            _currentEmotion = emotion;

            // 애니메이터 파라미터 사용 시
            if (!string.IsNullOrEmpty(_emotionParamName))
            {
                int emotionValue = (int)emotion;
                _animator.SetInteger(_emotionParamName, emotionValue);
            }

            // 강도 파라미터 사용 시
            if (!string.IsNullOrEmpty(_intensityParamName))
            {
                _animator.SetFloat(_intensityParamName, intensity);
            }

            // 애니메이션 클립 이름 사용 시
            string animationName = GetAnimationName(emotion);
            if (!string.IsNullOrEmpty(animationName))
            {
                _animator.CrossFadeInFixedTime(animationName, _crossFadeDuration);
            }

            Debug.Log($"[CharacterAnimationController] 감정 애니메이션 재생: {emotion} (강도: {intensity})");
        }

        /// <summary>
        /// 대화 중 상태 설정 (말하고 있는지)
        /// </summary>
        public void SetTalking(bool isTalking)
        {
            if (_animator == null)
            {
                return;
            }

            _isTalking = isTalking;

            if (!string.IsNullOrEmpty(_isTalkingParamName))
            {
                _animator.SetBool(_isTalkingParamName, isTalking);
            }
        }

        /// <summary>
        /// Idle 상태로 복귀
        /// </summary>
        public void ReturnToIdle()
        {
            SetEmotion(EmotionType.Neutral, 50);
            SetTalking(false);
        }

        /// <summary>
        /// 감정 설정 (애니메이션 재생)
        /// </summary>
        public void SetEmotion(EmotionType emotion, int intensity = 50)
        {
            float normalizedIntensity = intensity / 100f;
            PlayEmotionAnimation(emotion, normalizedIntensity);
        }

        /// <summary>
        /// 특정 애니메이션 재생
        /// </summary>
        public void PlayAnimation(string animationName)
        {
            if (_animator == null)
            {
                Debug.LogWarning("[CharacterAnimationController] Animator가 없습니다.");
                return;
            }

            if (string.IsNullOrEmpty(animationName))
            {
                Debug.LogWarning("[CharacterAnimationController] 애니메이션 이름이 비어있습니다.");
                return;
            }

            _animator.CrossFadeInFixedTime(animationName, _crossFadeDuration);
            Debug.Log($"[CharacterAnimationController] 애니메이션 재생: {animationName}");
        }

        /// <summary>
        /// 현재 재생 중인 애니메이션 이름 반환
        /// </summary>
        public string GetCurrentAnimationName()
        {
            if (_animator == null)
            {
                return "";
            }

            var currentClipInfo = _animator.GetCurrentAnimatorClipInfo(0);
            if (currentClipInfo.Length > 0)
            {
                return currentClipInfo[0].clip.name;
            }

            return "";
        }

        /// <summary>
        /// 애니메이터 속도 설정 (재생 속도 조절)
        /// </summary>
        public void SetAnimationSpeed(float speed)
        {
            if (_animator != null)
            {
                _animator.speed = speed;
            }
        }

        /// <summary>
        /// 감정별 애니메이션 이름 반환
        /// </summary>
        private string GetAnimationName(EmotionType emotion)
        {
            if (_animationNames == null)
            {
                return "";
            }

            return emotion switch
            {
                EmotionType.Happy => _animationNames.happy,
                EmotionType.Sad => _animationNames.sad,
                EmotionType.Angry => _animationNames.angry,
                EmotionType.Surprised => _animationNames.surprised,
                EmotionType.Thinking => _animationNames.thinking,
                EmotionType.Neutral => _animationNames.neutral,
                _ => ""
            };
        }
    }

    /// <summary>
    /// 감정별 애니메이션 클립 이름 매핑
    /// </summary>
    [System.Serializable]
    public class AnimationNameMapping
    {
        [Tooltip("기본 상태 애니메이션 이름")]
        public string neutral = "Idle";

        [Tooltip("행복 애니메이션 이름")]
        public string happy = "Happy";

        [Tooltip("슬픔 애니메이션 이름")]
        public string sad = "Sad";

        [Tooltip("화남 애니메이션 이름")]
        public string angry = "Angry";

        [Tooltip("놀람 애니메이션 이름")]
        public string surprised = "Surprised";

        [Tooltip("생각 중 애니메이션 이름")]
        public string thinking = "Thinking";
    }
}
