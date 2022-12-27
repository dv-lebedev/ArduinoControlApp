#include <math.h>
#include "RDecoder.h"
#include <Servo.h>

const int TONE_OUTPUT_PIN = 5;

const int TELEMETRY_SIZE = 8 * 16;
unsigned char telemetry[TELEMETRY_SIZE];
Servo myServo;

void setup()
{
  Serial.begin(128000);

  myServo.attach(9);

}

void loop()
{
    read();

    write();

    //myServo.write(angle);

     //delay(50);
}

void fill_telemetry(uint8_t analog_pin, int offset){
    uint16_t a0_val = (uint16_t)analogRead(analog_pin);
    telemetry[offset] = (uint8_t)((a0_val >> 8) & 0xFF);
    telemetry[offset + 1] = (uint8_t)(a0_val & 0xFF);
}

void write()
{
  fill_telemetry(A0, 0);
  fill_telemetry(A1, 2);
  fill_telemetry(A2, 4);
  fill_telemetry(A3, 6);

  ENCODED_MSG msg = encode(0x07, telemetry, TELEMETRY_SIZE);

  //Serial.write(msg.encoded_data, msg.size);
  free(msg.encoded_data);
}

const int READ_BUFFER_SIZE = 16;
unsigned char read_buf[READ_BUFFER_SIZE];

void handler(Package p){

  ENCODED_MSG msg = encode(0xCB, p.Data, p.data_size);
  Serial.write(msg.encoded_data, msg.size);
  
  if (p.Addr == 0xCB && p.data_size > 1)
  {
    uint16_t res = p.Data[0] << 8 | p.Data[1];

    //myServo.write((int)p.Data[0]);

    myServo.write(res);
  }

  if (p.Addr == 0xAA && p.data_size >= 8)
  {
    
    int freq = p.Data[0] << 24 | p.Data[1] << 16 | p.Data[2] << 8 | p.Data[3];
    int duration = p.Data[4] << 24 | p.Data[5] << 16 | p.Data[6] << 8 | p.Data[7];

    if (freq > 0 && duration > 0){
      tone(TONE_OUTPUT_PIN, freq, duration);
    }
  }

  free(msg.encoded_data);
  free(p.Data);

}


void read(){

if (Serial.available() > 0) {
    // read the incoming bytes:
    int rlen = Serial.readBytes(read_buf, READ_BUFFER_SIZE);

    decode(read_buf, 0, rlen, handler);

  }
}


