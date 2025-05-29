#include <M5Atom.h>
#include <WiFi.h>
#include <Arduino.h>

#include "udp_sender.h"

CRGB dispColor(uint8_t r, uint8_t g, uint8_t b) {
  return (CRGB)((r << 16) | (g << 8) | b);
}

const char *ssid = "TP-Link_B308";
const char *password = "29640393";

// 新美のMacBookのIPアドレス
const IPAddress targetIP(192, 168, 0, 140);
// 事前にsidのlsofで確認しておく
const int targetPort = 12345;
const char *message = "Hello from M5Atom!";

void setup() {
  M5.begin(true, false, true);
  delay(50);
  // 初期化中/WiFi未接続時は紫点灯
  M5.dis.drawpix(0, dispColor(255, 0, 255));
  delay(50);

  Serial.begin(115200);
  Serial.print("Starting WiFi connection to ");

  WiFi.begin(ssid, password);
  Serial.print(ssid);

  Serial.print("...");

  // WiFi接続を待つ
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }

  Serial.println("Connected to WiFi");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}

void loop() {
  M5.update();

  // WiFiの接続状態を確認
  if (WiFi.status() != WL_CONNECTED){
    // WiFiが接続されていない場合，紫色で点灯
    M5.dis.drawpix(0, dispColor(255, 0, 255));
    Serial.println("WiFi not connected");
  }else{
    // WiFiが接続されている場合，緑色で点灯
    M5.dis.drawpix(0, dispColor(0, 255, 0));
    Serial.println("WiFi connected");

    // UDPメッセージを送信
    sendUdpMessage(targetIP, targetPort, message);
  }

  delay(1000);
}