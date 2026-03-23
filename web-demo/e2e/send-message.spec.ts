import { test, expect } from '@playwright/test';

test.describe('메시지 전송 테스트', () => {
  test('input box에 "안녕"을 입력하고 전송 버튼 클릭 후 결과 캡쳐', async ({ page }) => {
    await page.goto('/');

    // input box에 "안녕" 입력
    const input = page.locator('#message-input');
    await expect(input).toBeVisible();
    await input.fill('안녕');

    // 전송 버튼 클릭
    const sendBtn = page.locator('#send-btn');
    await sendBtn.click();

    // 사용자 메시지가 화면에 표시되는지 확인
    const userMessage = page.locator('.user-message .message-bubble');
    await expect(userMessage.first()).toContainText('안녕');

    // AI 응답 메시지가 나타날 때까지 대기 (초기 환영 메시지 외 새 AI 메시지)
    // 초기 ai-message 1개 + 새 응답 1개 = 최소 2개
    const aiMessages = page.locator('.ai-message .message-bubble');
    await expect(aiMessages).toHaveCount(2, { timeout: 30000 });

    // 결과 화면 캡쳐
    await page.screenshot({ path: 'e2e/screenshots/send-message-result.png', fullPage: true });
  });
});
