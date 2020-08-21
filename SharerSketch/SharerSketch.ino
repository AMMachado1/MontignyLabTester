/*
 Name:		SharerSketch.ino
 Created:	08/01/20 2:15:24 PM
 Author:	Alberto Machado
*/

#include <HX711.h>
#include <Sharer.h>
#include <eeprom.h>

//Define Loadcell Pins
#define DOUT  4
#define CLK  5
HX711 Loadcell;
float calibration_factor;
float Loadcell_Mass;
uint16_t Cal_Weight;
//Servo Speed
int SpeedRef = 0;
//Encoder Variables
volatile long enctemp, encoder = 0;
//Share AI0 for Testing
int ana = 0;
//Servo Ready
bool Srdy;
//Servo Zero Speed
bool Szpd;


// Move Slide Up 
void SlideUp(void) {
	
	digitalWrite(8, false);
}

// Move Slide Up Stop 
void SlideUpStop(void) {
	digitalWrite(8, true);
}

// Move Slide Down 
void SlideDown(void) {
	digitalWrite(9, false);
	digitalWrite(8, false);
}

// Move Slide Down Stop 
void SlideDownStop(void) {
	digitalWrite(9, true);
	digitalWrite(8, true);
}

//Tare Loadcell
void Tare(void) {
	Loadcell.tare(); //Reset the scale to 0
}

//Calibrate Loadcell
void Calibrate(void) {
	Loadcell.callibrate_scale(Cal_Weight,5); //Calibrate the scale
	calibration_factor = Loadcell.get_scale();
	eeprom_write_float(0, calibration_factor);
	
}

//Set Scale of Loadcell
/*
void SetScale(void) {
	Loadcell.set_scale(calibration_factor); //Scale the Loadcell
}
*/
//Zero Encoder
void ZeroEnc(void) {
	encoder = 0;
}

void setup() {
	
	//Enable and Up
	pinMode(8, OUTPUT);
	//Down
	pinMode(9, OUTPUT);
	//Servo Speed Reference
	pinMode(6, OUTPUT);
	//Switch Both Realys On
	digitalWrite(9, true);
	digitalWrite(8, true);

	//Set Servo Status Pins
	pinMode(10, INPUT_PULLUP); 
	pinMode(11, INPUT_PULLUP);

	//Encoder Setup
	pinMode(2, INPUT_PULLUP); // internal pullup input pin 2 
	pinMode(3, INPUT_PULLUP); // internalเป็น pullup input pin 3
   //Setting up interrupt
	 //A rising pulse from encodenren activated ai0(). AttachInterrupt 0 is DigitalPin nr 2 on moust Arduino.
	attachInterrupt(0, ai0, RISING);
	//B rising pulse from encodenren activated ai1(). AttachInterrupt 1 is DigitalPin nr 3 on moust Arduino.
	attachInterrupt(1, ai1, RISING);
	//End Encoder Setup

	// Start Scale
	Loadcell.begin(DOUT, CLK);
	calibration_factor = eeprom_read_float(0);
	Loadcell.set_scale(calibration_factor);
	
	//long zero_factor = Loadcell.read_average(); //Get a baseline reading

	Sharer.init(38400); // Init Serial communication with 115200 bauds

	// Share the built-in analogRead function
	Sharer_ShareFunction(int, analogRead, uint8_t, pin);
	
	// Share the built-in digitalRead function
	Sharer_ShareFunction(bool, digitalRead, uint8_t, pin);

	// Expose this functions without returned parameters
	Sharer_ShareVoid(SlideUp);
	Sharer_ShareVoid(SlideUpStop);
	Sharer_ShareVoid(SlideDown);
	Sharer_ShareVoid(SlideDownStop);
	Sharer_ShareVoid(Tare);
	Sharer_ShareVoid(Calibrate);
	//Sharer_ShareVoid(SetScale);
	Sharer_ShareVoid(ZeroEnc);

	// Share variables for read/write from desktop application
	Sharer_ShareVariable(long, encoder);
	Sharer_ShareVariable(int, SpeedRef);
	//Sharer_ShareVariable(long, zero_factor);
	//Sharer_ShareVariable(float, calibration_factor);
	Sharer_ShareVariable(int, Cal_Weight);
	Sharer_ShareVariable(float, Loadcell_Mass);
	Sharer_ShareVariable(int, ana);
	Sharer_ShareVariable(bool, Srdy);
	Sharer_ShareVariable(bool, Szpd);
}

// Run Sharer engine in the main Loop
void loop() {
	Sharer.run();
	
	//Write Servo Speed Reference
	analogWrite(6, SpeedRef);

	//Read Analog Input, Pot for testing
	ana = analogRead(0);

	//Read Servo Ready
	Srdy = digitalRead(10);

	//Read Servo Zero Speed
	Szpd = digitalRead(11);

	//Encoder
	// Send the value of counter
	if (encoder != enctemp) {
	//	//Serial.println (counter);
		enctemp = encoder;
	}
	//End Encoder

	//Read Loadcell
	Loadcell_Mass = Loadcell.get_units(5);
	calibration_factor = Loadcell.get_scale();
}

void ai0() {
	// ai0 is activated if DigitalPin nr 2 is going from LOW to HIGH
	// Check pin 3 to determine the direction
	if (digitalRead(3) == LOW) {
		encoder++;
	}
	else {
		encoder--;
	}
}

void ai1() {
	// ai0 is activated if DigitalPin nr 3 is going from LOW to HIGH
	// Check with pin 2 to determine the direction
	if (digitalRead(2) == LOW) {
		encoder--;
	}
	else {
		encoder++;
	}
}
