#ifndef UDP_SENDER_H
#define UDP_SENDER_H

#include <WiFi.h>
#include <WiFiUdp.h>

void sendUdpMessage(IPAddress targetIP, int targetPort, const char* message);

#endif //UDP_SENDER_H