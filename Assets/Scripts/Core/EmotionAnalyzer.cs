using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AISpouse.Core
{
    public class EmotionAnalyzer
    {
        private const string EMOTION_TAG_PATTERN = @"\[(\w+)\]";

        // 감정 태그 문자열과 EmotionType 매핑
        private static readonly Dictionary<string, EmotionType> EMOTION_MAP = new Dictionary<string, EmotionType>
        {
            { "happy", EmotionType.Happy },
            { "happiness", EmotionType.Happy },
            { "joy", EmotionType.Happy },
            { "smile", EmotionType.Happy },
            { "sad", EmotionType.Sad },
            { "sadness", EmotionType.Sad },
            { "cry", EmotionType.Sad },
            { "angry", EmotionType.Angry },
            { "anger", EmotionType.Angry },
            { "mad", EmotionType.Angry },
            { "surprised", EmotionType.Surprised },
            { "surprise", EmotionType.Surprised },
            { "shocked", EmotionType.Surprised },
            { "thinking", EmotionType.Thinking },
            { "think", EmotionType.Thinking },
            { "wonder", EmotionType.Thinking },
            { "neutral", EmotionType.Neutral },
            { "normal", EmotionType.Neutral }
        };

        /// <summary>
        /// AI 응답에서 감정 태그를 추출하고 EmotionData를 반환
        /// </summary>
        public EmotionData AnalyzeEmotion(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return new EmotionData(EmotionType.Neutral, 50, "", "");
            }

            EmotionType detectedEmotion = EmotionType.Neutral;
            int maxIntensity = 0;

            // 모든 감정 태그를 찾음
            var matches = Regex.Matches(response, EMOTION_TAG_PATTERN, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string tag = match.Groups[1].Value.ToLower();

                if (EMOTION_MAP.TryGetValue(tag, out EmotionType emotion))
                {
                    // 첫 번째 감정 태그를 기본 감정으로 사용 (여러 태그가 있을 경우)
                    if (detectedEmotion == EmotionType.Neutral)
                    {
                        detectedEmotion = emotion;
                    }

                    // 태그가 여러 개 있을 경우 강도 증가
                    maxIntensity = Math.Min(100, maxIntensity + 25);
                }
            }

            // 감정 태그가 없는 경우 기본 감정 강도는 50
            if (maxIntensity == 0)
            {
                maxIntensity = 50;
            }

            // 감정 태그 제거된 텍스트
            string rawText = RemoveEmotionTags(response);

            EmotionData result = new EmotionData(
                detectedEmotion,
                maxIntensity,
                rawText,
                response
            );

            Debug.Log($"[EmotionAnalyzer] 감정 분석: {detectedEmotion}, 강도: {maxIntensity}");
            return result;
        }

        /// <summary>
        /// 텍스트에서 감정 태그를 제거
        /// </summary>
        public string RemoveEmotionTags(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string cleaned = Regex.Replace(text, EMOTION_TAG_PATTERN, "", RegexOptions.IgnoreCase);

            // 공백 정리
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

            return cleaned;
        }

        /// <summary>
        /// 감정 태그가 있는지 확인
        /// </summary>
        public bool HasEmotionTags(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return Regex.IsMatch(text, EMOTION_TAG_PATTERN, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 텍스트에서 모든 감정 태그를 추출하여 반환
        /// </summary>
        public List<EmotionType> ExtractAllEmotions(string text)
        {
            List<EmotionType> emotions = new List<EmotionType>();

            if (string.IsNullOrEmpty(text))
            {
                return emotions;
            }

            var matches = Regex.Matches(text, EMOTION_TAG_PATTERN, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string tag = match.Groups[1].Value.ToLower();

                if (EMOTION_MAP.TryGetValue(tag, out EmotionType emotion))
                {
                    emotions.Add(emotion);
                }
            }

            return emotions;
        }
    }
}
