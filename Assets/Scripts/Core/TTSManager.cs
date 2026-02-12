using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AISpouse.Core
{
    public enum TTSProvider
    {
        OpenAI,
        ElevenLabs
    }

    public class TTSManager : MonoBehaviour
    {
        [Header("TTS 공급자 선택")]
        [SerializeField]
        private TTSProvider _ttsProvider = TTSProvider.OpenAI;

        [Header("OpenAI TTS 설정")]
        [SerializeField]
        private string _openAIKey;

        [SerializeField]
        private string _openAIVoiceName = "nova"; // nova: 여성 목소리, alloy, echo, fable, onyx, shimmer

        [SerializeField]
        private string _openAIApiEndpoint = "https://api.openai.com/v1/audio/speech";

        [Header("ElevenLabs TTS 설정")]
        [SerializeField]
        private string _elevenLabsKey = "sk_dedafa1b99e3238daf8ab4e6988ae8722930f1196d7cf6c1";

        [SerializeField]
        private string _elevenLabsModelId = "eleven_v3";

        [SerializeField]
        private string _elevenLabsVoiceId = "21m00Tcm4TlvDq8ikWAM"; // 기본 목소리 ID

        [SerializeField]
        private string _elevenLabsApiEndpoint = "https://api.elevenlabs.io/v1/text";

    [Header("기본 설정")]
        [SerializeField]
        private float _defaultSpeed = 1.0f;

        [SerializeField]
        private int _requestTimeoutSeconds = 30;

        [Header("감정별 피치 설정")]
        [SerializeField]
        private float _happyPitch = 1.2f;

        [SerializeField]
        private float _sadPitch = 0.8f;

        [SerializeField]
        private float _angryPitch = 1.3f;

        [SerializeField]
        private float _surprisedPitch = 1.4f;

        private AudioSource _audioSource;
        private bool _isSpeaking = false;
        private bool _stopRequested = false;
        private Coroutine _currentPlaybackCoroutine;

        public event Action OnSpeakingStarted;
        public event Action OnSpeakingFinished;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        private void Start()
        {
            // GameConfig에서 API 키 가져오기 시도
            var gameConfig = FindObjectOfType<GameConfig>();
            if (gameConfig != null)
            {
                if (!string.IsNullOrEmpty(gameConfig.OpenAIKey))
                {
                    _openAIKey = gameConfig.OpenAIKey;
                    Debug.Log("[TTSManager] GameConfig에서 OpenAI 키 로드됨");
                }
            }

            // API 키 확인
            if (_ttsProvider == TTSProvider.OpenAI && string.IsNullOrEmpty(_openAIKey))
            {
                Debug.LogWarning("[TTSManager] OpenAI API 키가 설정되지 않았습니다!");
            }
            else if (_ttsProvider == TTSProvider.ElevenLabs && string.IsNullOrEmpty(_elevenLabsKey))
            {
                Debug.LogWarning("[TTSManager] ElevenLabs API 키가 설정되지 않았습니다!");
            }

            Debug.Log($"[TTSManager] TTS 공급자: {_ttsProvider}");
        }

        /// <summary>
        /// 텍스트를 음성으로 재생
        /// </summary>
        public async void SpeakText(string text, EmotionType emotion = EmotionType.Neutral)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("[TTSManager] 텍스트가 비어있습니다.");
                return;
            }

            if (string.IsNullOrEmpty(_openAIKey))
            {
                Debug.LogError("[TTSManager] OpenAI API 키가 설정되지 않았습니다!");
                return;
            }

            // 현재 재생 중인 경우 중지
            StopSpeaking();

            // 약간의 딜레이 후 시작
            await Task.Delay(100);

            if (_stopRequested)
            {
                return;
            }

            try
            {
                byte[] audioData;

                if (_ttsProvider == TTSProvider.OpenAI)
                {
                    audioData = await GenerateOpenAISpeechAsync(text, emotion);
                }
                else if (_ttsProvider == TTSProvider.ElevenLabs)
                {
                    audioData = await GenerateElevenLabsSpeechAsync(text, emotion);
                }
                else
                {
                    audioData = null;
                }

                if (audioData != null && audioData.Length > 0)
                {
                    PlayAudioClip(audioData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TTSManager] 음성 생성 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// OpenAI TTS API를 사용하여 음성 생성
        /// </summary>
        private async Task<byte[]> GenerateOpenAISpeechAsync(string text, EmotionType emotion)
        {
            if (_stopRequested)
            {
                return null;
            }

            string requestBody = $"{{\"model\":\"tts-1\",\"input\":\"{EscapeJsonString(text)}\",\"voice\":\"{_openAIVoiceName}\",\"speed\":{_defaultSpeed}}}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);

            using (UnityWebRequest request = new UnityWebRequest(_openAIApiEndpoint, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {_openAIKey}");
                request.timeout = _requestTimeoutSeconds;

                Debug.Log($"[TTSManager] OpenAI TTS 요청: {text.Substring(0, Math.Min(30, text.Length))}...");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    if (_stopRequested)
                    {
                        request.Abort();
                        return null;
                    }
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.data;
                }
                else
                {
                    Debug.LogError($"[TTSManager] OpenAI TTS API 요청 실패: {request.error}");
                    Debug.LogError($"[TTSManager] 응답: {request.downloadHandler.text}");
                    return null;
                }
            }
        }

        /// <summary>
        /// ElevenLabs TTS API를 사용하여 음성 생성
        /// </summary>
        private async Task<byte[]> GenerateElevenLabsSpeechAsync(string text, EmotionType emotion)
        {
            if (_stopRequested)
            {
                return null;
            }

            string url = $"https://api.elevenlabs.io/v1/text-to-speech/{_elevenLabsVoiceId}";

            // 감정에 따른 stability 및 similarity_boost 설정
            float stability = GetEmotionStability(emotion);
            float similarityBoost = GetEmotionSimilarityBoost(emotion);

            string requestBody = $"{{\"text\":\"{EscapeJsonString(text)}\",\"model_id\":\"{_elevenLabsModelId}\",\"voice_settings\":{{\"stability\":{stability:F2},\"similarity_boost\":{similarityBoost:F2}}}}}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("xi-api-key", _elevenLabsKey);
                request.timeout = _requestTimeoutSeconds;

                Debug.Log($"[TTSManager] ElevenLabs TTS 요청: {text.Substring(0, Math.Min(30, text.Length))}...");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    if (_stopRequested)
                    {
                        request.Abort();
                        return null;
                    }
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.data;
                }
                else
                {
                    Debug.LogError($"[TTSManager] ElevenLabs TTS API 요청 실패: {request.error}");
                    Debug.LogError($"[TTSManager] 응답: {request.downloadHandler.text}");
                    return null;
                }
            }
        }

        /// <summary>
        /// 오디오 데이터 재생
        /// </summary>
        private void PlayAudioClip(byte[] audioData)
        {
            if (_stopRequested)
            {
                return;
            }

            _isSpeaking = true;
            OnSpeakingStarted?.Invoke();

            AudioClip clip = NAudioPlayer.LoadAudioClip(audioData);

            if (clip != null)
            {
                _currentPlaybackCoroutine = StartCoroutine(PlayClipCoroutine(clip));
            }
            else
            {
                Debug.LogError("[TTSManager] 오디오 클립 생성 실패");
                _isSpeaking = false;
                OnSpeakingFinished?.Invoke();
            }
        }

        /// <summary>
        /// 오디오 클립 재생 코루틴
        /// </summary>
        private IEnumerator PlayClipCoroutine(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();

            while (_audioSource.isPlaying && !_stopRequested)
            {
                yield return null;
            }

            _audioSource.Stop();
            Destroy(clip);

            _isSpeaking = false;

            if (!_stopRequested)
            {
                OnSpeakingFinished?.Invoke();
            }

            _currentPlaybackCoroutine = null;
        }

        /// <summary>
        /// 현재 재생 중지
        /// </summary>
        public void StopSpeaking()
        {
            if (_isSpeaking)
            {
                _stopRequested = true;

                if (_audioSource != null)
                {
                    _audioSource.Stop();
                }

                if (_currentPlaybackCoroutine != null)
                {
                    StopCoroutine(_currentPlaybackCoroutine);
                    _currentPlaybackCoroutine = null;
                }

                _isSpeaking = false;
                _stopRequested = false;

                OnSpeakingFinished?.Invoke();

                Debug.Log("[TTSManager] 음성 재생 중지");
            }
        }

        /// <summary>
        /// 재생 중인지 확인
        /// </summary>
        public bool IsSpeaking()
        {
            return _isSpeaking;
        }

        /// <summary>
        /// 볼륨 설정
        /// </summary>
        public void SetVolume(float volume)
        {
            if (_audioSource != null)
            {
                _audioSource.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// 감정별 피치 계산
        /// </summary>
        private float GetEmotionPitch(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Happy => _happyPitch,
                EmotionType.Sad => _sadPitch,
                EmotionType.Angry => _angryPitch,
                EmotionType.Surprised => _surprisedPitch,
                _ => 1.0f
            };
        }

        /// <summary>
        /// 감정별 ElevenLabs stability 계산
        /// </summary>
        private float GetEmotionStability(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Happy => 0.3f,  // 더 다양한 표현
                EmotionType.Sad => 0.7f,     // 더 안정적
                EmotionType.Angry => 0.2f,   // 더 다양하고 강렬
                EmotionType.Surprised => 0.25f,
                EmotionType.Thinking => 0.6f,
                _ => 0.5f
            };
        }

        /// <summary>
        /// 감정별 ElevenLabsari similarity_boost 계산
        /// </summary>
        private float GetEmotionSimilarityBoost(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Happy => 0.8f,
                EmotionType.Sad => 0.75f,
                EmotionType.Angry => 0.85f,
                EmotionType.Surprised => 0.9f,
                EmotionType.Thinking => 0.7f,
                _ => 0.75f
            };
        }

        /// <summary>
        /// JSON 문자열 이스케이프
        /// </summary>
        private string EscapeJsonString(string text)
        {
            return text.Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace("\t", "\\t");
        }
    }

    /// <summary>
    /// 오디오 데이터에서 AudioClip을 생성하는 정적 클래스
    /// </summary>
    public static class NAudioPlayer
    {
        public static AudioClip LoadAudioClip(byte[] wavData)
        {
            // WAV 파일 헤더 파싱
            // 44바이트 헤더 후에 실제 오디오 데이터 시작

            if (wavData == null || wavData.Length < 44)
            {
                return null;
            }

            try
            {
                int headerSize = 44;
                int audioDataLength = wavData.Length - headerSize;

                // WAV 헤더에서 채널, 샘플레이트, 비트 뎁스 읽기
                // Little Endian
                int channels = wavData[22] | (wavData[23] << 8);
                int sampleRate = wavData[24] | (wavData[25] << 8) | (wavData[26] << 16) | (wavData[27] << 24);
                int bitsPerSample = wavData[34] | (wavData[35] << 8);

                // 오디오 데이터 추출
                float[] audioData = ConvertWavToFloatArray(wavData, headerSize, audioDataLength, channels, bitsPerSample);

                AudioClip clip = AudioClip.Create(
                    "TTS_Clip",
                    audioData.Length / channels,
                    channels,
                    sampleRate,
                    false
                );

                clip.SetData(audioData, 0);

                return clip;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NAudioPlayer] 오디오 데이터 변환 오류: {ex.Message}");
                return null;
            }
        }

        private static float[] ConvertWavToFloatArray(byte[] wavData, int offset, int length, int channels, int bitsPerSample)
        {
            int sampleCount = length / (bitsPerSample / 8);
            float[] audioData = new float[sampleCount];

            if (bitsPerSample == 16)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    short sample = (short)(wavData[offset + i * 2] | (wavData[offset + i * 2 + 1] << 8));
                    audioData[i] = sample / 32768f; // 정규화 (-1 ~ 1)
                }
            }
            else if (bitsPerSample == 24)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    int sample = wavData[offset + i * 3] |
                                 (wavData[offset + i * 3 + 1] << 8) |
                                 (wavData[offset + i * 3 + 2] << 16);

                    if (sample >= 0x800000)
                    {
                        sample = sample - 0x1000000;
                    }

                    audioData[i] = sample / 8388608f; // 정규화 (-1 ~ 1)
                }
            }
            else if (bitsPerSample == 32)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    int sample = wavData[offset + i * 4] |
                                 (wavData[offset + i * 4 + 1] << 8) |
                                 (wavData[offset + i * 4 + 2] << 16) |
                                 (wavData[offset + i * 4 + 3] << 24);

                    audioData[i] = sample / 2147483648f; // 정규화 (-1 ~ 1)
                }
            }

            return audioData;
        }
    }
}
