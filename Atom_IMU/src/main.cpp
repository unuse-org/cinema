#include <M5Unified.h>

void setup() {
  auto cfg = M5.config();
  cfg.clear_display = false;
  M5.begin(cfg);

  Serial.begin(115200);
  while (!Serial);

  if (!M5.Imu.isEnabled()) {
    Serial.println("IMU not available!");
    return;
  }

  Serial.println("MPU6886 ready.");
}

int getStatusFromAccel(float az) {
  // 範囲設定
  float range_H_max = 0.90, range_H_min = 0.80;
  float range_L_max = 0.60, range_L_min = 0.40;

  if (az < range_H_max && az > range_H_min) return 1;
  else if (az < range_L_max && az > range_L_min) return 2;
  else if (az > -range_L_max && az < -range_L_min) return 4;
  else if (az > -range_H_max && az < -range_H_min) return 5;
  else return 3;
}

void loop() {
  float ax, ay, az;
  int status = 0; // 0: upright, 1: upside down, 2: tilted

  // IMUデータの更新
  M5.Imu.getAccel(&ax, &ay, &az);
  // IMUデータの取得(デバッグ用)
  Serial.printf("Accel: X=%.2f, Y=%.2f, Z=%.2f (g)\n", ax, ay, az);

  status = getStatusFromAccel(az);

  // 状態をシリアルに出力
  Serial.print("Status: ");
  Serial.println(status);
  delay(500);
}