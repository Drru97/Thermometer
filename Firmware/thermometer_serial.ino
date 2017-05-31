#include <OneWire.h>
#include <DallasTemperature.h>
#include <SoftwareSerial.h>

const int sensorPin = 7;
const int txPin = 2;
const int rxPin = 3;

SoftwareSerial BT(txPin, rxPin);
OneWire oneWire(sensorPin);
DallasTemperature sensors(&oneWire);

void setup()
{
  Serial.begin(9600);
  BT.begin(9600);
  Serial.setTimeout(1000);
  BT.setTimeout(1000);
  sensors.begin();
}

void loop()
{
  sensors.requestTemperatures();
  float temp = floorf(sensors.getTempCByIndex(0) * 100) / 100; // 2 decimals after point
  Serial.println(temp);
  Serial.flush();
  BT.println(temp);
  BT.flush();
  delay(500);
}

