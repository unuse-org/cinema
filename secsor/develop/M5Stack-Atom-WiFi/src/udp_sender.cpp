#include "udp_sender.h"
#include <Arduino.h> // Serial.printlnaのために必要

WiFiUDP udp;

void sendUdpMessage(IPAddress targetIP, int targetPort, const char* message){
    Serial.print("Sending UDP message to ");
    Serial.print(targetIP);
    Serial.print(":");
    Serial.println(targetPort);
    Serial.print("Message: ");
    Serial.println(message);

    // UDPパケットの送信を開始
    if (udp.beginPacket(targetIP, targetPort) == 1) {
        udp.write((const uint8_t*)message, strlen(message));

        udp.endPacket();
    } else {
        Serial.println("Err: Failed to begin UDP packet");
    }
}