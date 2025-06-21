#include <M5Unified.h>
#include "Adafruit_TCS34725.h"

// カラーセンサの初期化（統合時間とゲイン設定）
Adafruit_TCS34725 tcs = Adafruit_TCS34725(
    TCS34725_INTEGRATIONTIME_154MS,
    TCS34725_GAIN_4X);

// 表示用のRGB（スケーリング済み）
int r_value = 0, g_value = 0, b_value = 0;
int flag = -1;

void setup() {
  auto cfg = M5.config();
  M5.begin(cfg);

  Serial.begin(115200);

  M5.Display.setRotation(1);
  M5.Display.fillScreen(TFT_DARKGREY);
  M5.Display.setTextSize(2);
  M5.Display.setCursor(10, 10);
  M5.Display.setTextColor(TFT_WHITE);
  M5.Display.println("Color Sensor Display");

  while (!tcs.begin()) {
    M5.Display.setCursor(10, 40);
    M5.Display.println("No sensor found");
    delay(1000);
  }
}

// RGB値とカラーパッチをディスプレイに表示する
void drawRGBValues(int r, int g, int b) {
  M5.Display.fillRect(0, 50, 320, 150, TFT_DARKGREY);

  M5.Display.setTextColor(TFT_RED);
  M5.Display.setCursor(20, 60);
  M5.Display.printf("R: %3d", r);

  M5.Display.setTextColor(TFT_GREEN);
  M5.Display.setCursor(20, 100);
  M5.Display.printf("G: %3d", g);

  M5.Display.setTextColor(TFT_BLUE);
  M5.Display.setCursor(20, 140);
  M5.Display.printf("B: %3d", b);

  uint16_t rgbColor = M5.Display.color565(r, g, b);
  M5.Display.fillRect(180, 60, 120, 100, rgbColor);
  M5.Display.drawRect(180, 60, 120, 100, TFT_WHITE);
}

void loop() {
  uint16_t clear, red_raw, green_raw, blue_raw;
  tcs.getRawData(&red_raw, &green_raw, &blue_raw, &clear);

  // 最大値を基準にスケーリング（0–255に）
  uint16_t max_val = max(red_raw, max(green_raw, blue_raw));
  float scale = (max_val > 0) ? 255.0 / max_val : 1.0;

  r_value = (int)(red_raw * scale);
  g_value = (int)(green_raw * scale);
  b_value = (int)(blue_raw * scale);

  // 色の判定
  flag = -1;
  if (r_value > 240) flag = 0;
  if (g_value > 240) flag = 1;
  if (b_value > 240) flag = 2;

  // 表示と出力
  drawRGBValues(r_value, g_value, b_value);

  // Serial.printf("R:%3d G:%3d B:%3d => ", r_value, g_value, b_value);
  switch (flag) {
    case 0: Serial.println("0"); break;
    case 1: Serial.println("1"); break;
    // case 2: Serial.println("2"); break;
    default: Serial.println("flag: NONE");
  }

  delay(1000);
}
