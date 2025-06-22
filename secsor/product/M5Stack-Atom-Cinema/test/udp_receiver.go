package main

import (
	"fmt"
	"net"
)

const (
	// 受信待ちするIPアドレス
	listenIP = "0.0.0.0"
	// 受信待ちするポート番号
	listenPort = 12345
	// 受信バッファのサイズ
	bufferSize = 1024
)

func main() {
	// UDPアドレスを作成
	address := fmt.Sprintf("%s:%d", listenIP, listenPort)

	listener, err := net.ListenPacket("udp", address)
	if err != nil {
		fmt.Printf("Error listening on UDP: %v\n", err)
		return
	}
	defer listener.Close()

	fmt.Printf("Listening for UDP packets on %s...\n", address)

	buffer := make([]byte, bufferSize)

	// 受信ループ
	for {
		n, addr, err := listener.ReadFrom(buffer)
		if err != nil {
			fmt.Printf("Error reading UDP packet: %v\n", err)
			continue
		}

		message := string(buffer[:n])

		fmt.Printf("Received message: %s from %s\n", message, addr.String())
	}
}
