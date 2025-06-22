#include <M5Unified.h>
#include <WiFi.h>
#include "Adafruit_TCS34725.h"
#include "udp_sender.h"

// -------- ユーザー設定項目 --------
// 接続するWi-FiのSSIDとパスワード
const char *ssid = "TP-Link_B308";
const char *password = "29640393";

// 送信先のIPアドレスとポート番号
const IPAddress targetIP(192, 168, 0, 140); // 受信側PCのIPアドレス
const int targetPort = 12345;
// ------------------------------------

// カラーセンサーの初期化
// M5Stack FireのGroveポートA (G21:SDA, G22:SCL) に接続
Adafruit_TCS34725 tcs = Adafruit_TCS34725(
    TCS34725_INTEGRATIONTIME_154MS,
    TCS34725_GAIN_4X);

// グローバル変数
// 表示用のRGB（スケーリング済み）
int r_value = 0, g_value = 0, b_value = 0;
int flag = -1;

void setup() {
  auto cfg = M5.config();
  M5.begin(cfg);

  // LCDの初期設定
  M5.Lcd.setRotation(1);
  M5.Lcd.setTextSize(2);
  M5.Lcd.fillScreen(TFT_BLACK);

  Serial.begin(115200);

  // --- カラーセンサーの初期化 ---
  M5.Lcd.setCursor(10, 10);
  M5.Lcd.println("TCS34725 Test");
  if (tcs.begin()) {
    Serial.println("Found color sensor!");
    M5.Lcd.println("Sensor Found!");
  } else {
    Serial.println("No TCS34725 found ... check connections");
    M5.Lcd.setTextColor(TFT_RED);
    M5.Lcd.println("Sensor NOT Found!");
    while (1); // センサーが見つからない場合は停止
  }
  delay(1000);

  // --- Wi-Fi接続処理 ---
  M5.Lcd.println("Connecting to WiFi...");
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    M5.Lcd.print(".");
  }
  Serial.println("\nWiFi connected!");
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  M5.Lcd.fillScreen(TFT_BLACK);
  M5.Lcd.setCursor(10,10);
  M5.Lcd.printf("WiFi Connected!\nIP: %s", WiFi.localIP().toString().c_str());
  delay(2000);
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
  M5.update(); // ボタンの状態などを更新

  if (WiFi.status() != WL_CONNECTED) {
    M5.Lcd.fillScreen(TFT_RED);
    M5.Lcd.setCursor(10, 10);
    M5.Lcd.setTextColor(TFT_WHITE);
    M5.Lcd.setTextSize(2);
    M5.Lcd.println("WiFi Disconnected");
    Serial.println("WiFi Disconnected");
    delay(1000);
    return;
  }
  
  // カラーセンサーから値を取得 (R, G, B, Clear)
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

  // 送信するメッセージをJSON形式で作成
  char message[128];

  // Serial.printf("R:%3d G:%3d B:%3d => ", r_value, g_value, b_value);
  switch (flag) {
    case 0: Serial.println("0"); break;
    case 1: Serial.println("1"); break;
    // case 2: Serial.println("2"); break;
    default: Serial.println("flag: NONE");
  }

  // flagをキャストしてchar型に変換
  char flagChar = (flag >= 0) ? '0' + flag : '-';

  // UDPでflagcharのみを送信
  snprintf(message, sizeof(message), "%c", flagChar);
  // 送信先のIPアドレスとポート番号にメッセージを送信
  sendUdpMessage(targetIP, targetPort, message);

  delay(500); // 0.5秒ごとに送信
}