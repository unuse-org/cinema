#include <M5Unified.h>
#include <WiFi.h>
#include "udp_sender.h"

// UDPブロードキャスト送信用の関数を定義
#include <WiFiUdp.h>
void sendUdpBroadcast(int port, const char* message) {
  WiFiUDP udp;
  udp.beginPacket("255.255.255.255", port);
  udp.write((const uint8_t*)message, strlen(message));
  udp.endPacket();
}

// LEDマトリックスの色を指定しやすくするヘルパー関数
uint32_t dispColor(uint8_t r, uint8_t g, uint8_t b) {
  return (r << 16) | (g << 8) | b;
}

// -------- ユーザー設定項目 --------
const char *ssid = "TP-Link_B308";
const char *password = "29640393";

const int targetPort = 12347;
// ------------------------------------


void setup() {
  auto cfg = M5.config();
  M5.begin(cfg);

  // 【修正】LEDマトリックスを初期化中/Wi-Fi未接続を示す紫色に点灯
  M5.Display.fillScreen(dispColor(128, 0, 128));
  delay(50);

  Serial.begin(115200);
  // while (!Serial);

  Serial.print("Starting WiFi connection to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }

  Serial.println("\nConnected to WiFi");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  if (!M5.Imu.isEnabled()) {
    Serial.println("IMU not available!");
    // 【修正】IMUが使えない場合はLEDマトリックスを赤色にして停止
    M5.Display.fillScreen(dispColor(255, 0, 0));
    while(1) { delay(100); }
  }
  
  Serial.println("IMU (BMI270) ready.");
}

int getSpeedFromAccel(float ax)
{
  if (ax < -0.90)
    return 3; // 高速
  else if (-0.30 < ax && -0.10 > ax)
    return 1; // 低速
  else 
    return 2; // 中速
}

void loop() {
  M5.update();

  if (WiFi.status() != WL_CONNECTED){
    // 【修正】Wi-Fiが切断された場合、LEDマトリックスを紫色にしてエラー表示
    M5.Display.fillScreen(dispColor(128, 0, 128));
    Serial.println("WiFi not connected");
  } else {
    // 【修正】Wi-Fiが接続されている場合、LEDマトリックスを緑色に点灯
    M5.Display.fillScreen(dispColor(0, 128, 0));

    float ax, ay, az;
    M5.Imu.getAccel(&ax, &ay, &az);
    
    Serial.printf("Accel: X=%.2f, Y=%.2f, Z=%.2f (g)\n", ax, ay, az);

    int speed = getSpeedFromAccel(ax);
    Serial.printf("Speed: %d\n", speed);

    char message[64];
    sprintf(message, "%d", speed);
    
    // ブロードキャスト送信
    sendUdpBroadcast(targetPort, message);

  }

  delay(500);
}