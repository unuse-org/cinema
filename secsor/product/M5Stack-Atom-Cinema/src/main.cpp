#include <M5Unified.h>
#include <WiFi.h>
#include "udp_sender.h"

// LEDマトリックスの色を指定しやすくするヘルパー関数
uint32_t dispColor(uint8_t r, uint8_t g, uint8_t b) {
  return (r << 16) | (g << 8) | b;
}

// -------- ユーザー設定項目 --------
const char *ssid = "TP-Link_B308";
const char *password = "29640393";
const IPAddress targetIP(192, 168, 0, 140);
const int targetPort = 12345;
// ------------------------------------


void setup() {
  auto cfg = M5.config();
  M5.begin(cfg);

  // 【修正】LEDマトリックスを初期化中/Wi-Fi未接続を示す紫色に点灯
  M5.Display.fillScreen(dispColor(128, 0, 128));
  delay(50);

  Serial.begin(115200);
  while (!Serial);

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

// 加速度センサーのZ軸の値からデバイスの状態を判定する関数
int getStatusFromAccel(float az) {
  const float range_H_max = 0.90, range_H_min = 0.80;
  const float range_L_max = 0.60, range_L_min = 0.40;

  if (az < range_H_max && az > range_H_min) return 1;
  else if (az < range_L_max && az > range_L_min) return 2;
  else if (az > -range_L_max && az < -range_L_min) return 4;
  else if (az > -range_H_max && az < -range_H_min) return 5;
  else return 3;
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

    int status = getStatusFromAccel(az);
    Serial.printf("Status: %d\n", status);

    char message[64];
    sprintf(message, "Status: %d", status);
    
    sendUdpMessage(targetIP, targetPort, message);
  }

  delay(500);
}