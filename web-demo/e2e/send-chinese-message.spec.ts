import { test, expect } from '@playwright/test';

test.describe('中文消息发送测试', () => {
  test('输入"宝贝，今天想我没"并点击发送按钮，验证返回信息并截图', async ({ page }) => {
    await page.goto('/');

    // 在输入框填写中文消息
    const input = page.locator('#message-input');
    await expect(input).toBeVisible();
    await input.fill('宝贝，今天想我没');

    // 点击发送按钮
    const sendBtn = page.locator('#send-btn');
    await sendBtn.click();

    // 验证用户消息显示在页面上
    const userMessage = page.locator('.user-message .message-bubble');
    await expect(userMessage.first()).toContainText('宝贝，今天想我没');

    // 等待AI返回消息（初始欢迎消息1个 + 新AI回复1个 = 2个）
    const aiMessages = page.locator('.ai-message .message-bubble');
    await expect(aiMessages).toHaveCount(2, { timeout: 30000 });

    // 验证AI回复不为空
    const aiReply = aiMessages.nth(1);
    await expect(aiReply).not.toBeEmpty();

    // 截图保存
    await page.screenshot({ path: 'e2e/screenshots/send-chinese-message-result.png', fullPage: true });
  });
});
