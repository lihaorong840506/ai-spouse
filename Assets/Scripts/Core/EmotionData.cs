namespace AISpouse.Core
{
    public class EmotionData
    {
        public EmotionType Emotion { get; set; }
        public int Intensity { get; set; } // 0~100
        public string RawText { get; set; } // 감정 태그 제거된 텍스트
        public string OriginalText { get; set; } // 원본 텍스트

        public EmotionData(EmotionType emotion, int intensity, string rawText, string originalText)
        {
            Emotion = emotion;
            Intensity = intensity;
            RawText = rawText;
            OriginalText = originalText;
        }
    }
}
