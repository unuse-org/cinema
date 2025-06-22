#include "udp_sender.h"
#include <Arduino.h>

WiFiUDP udp;

void sendUdpMessage(IPAddress targetIP, int targetPort, const char* message){
    Serial.print("Sending UDP: ");
    Serial.println(message);

    if (udp.beginPacket(targetIP, targetPort) == 1) {
        udp.write((const uint8_t*)message, strlen(message));
        udp.endPacket();
    } else {
        Serial.println("Err: Failed to begin UDP packet");
    }
}