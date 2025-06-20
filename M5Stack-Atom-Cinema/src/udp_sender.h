#ifndef UDP_SENDER_H
#define UDP_SENDER_H

#include <WiFi.h>
#include <WiFiUdp.h>

// UDPでメッセージを送信する関数の宣言
// Parameters:
// - targetIP: 通信対象のIPアドレス 
// - targetPort: 通信対象のポート番号
// - message: 通信するメッセージ
void sendUdpMessage(IPAddress targetIP, int targetPort, const char* message);

#endif