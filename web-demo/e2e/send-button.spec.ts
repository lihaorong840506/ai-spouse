import { test, expect } from '@playwright/test';

test.describe('전송 버튼 확인', () => {
  test('화면에 전송 버튼(#send-btn)이 존재해야 한다', async ({ page }) => {
    await page.goto('/');

    const sendBtn = page.locator('#send-btn');
    await expect(sendBtn).toBeVisible();
  });

  test('전송 버튼이 클릭 가능해야 한다', async ({ page }) => {
    await page.goto('/');

    const sendBtn = page.locator('#send-btn');
    await expect(sendBtn).toBeEnabled();
  });

  test('전송 버튼 안에 SVG 아이콘이 있어야 한다', async ({ page }) => {
    await page.goto('/');

    const svg = page.locator('#send-btn svg');
    await expect(svg).toBeVisible();
  });
});
