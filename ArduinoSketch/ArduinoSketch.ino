#include <Sharer.h>

int an_integer = 12;
bool a_boolean = true;
long a_long = 0;
int SpeedRef = 0;

// Turn built-in board LED on
void TurnLEDOn(void) {
	pinMode(13, OUTPUT);
	digitalWrite(13, true);
}

// Turn built-in board LED off
void TurnLEDOff(void) {
  pinMode(13, OUTPUT);
  digitalWrite(13, false);
}

// Move Slide Up 
void SlideUp(void) {
  pinMode(6, OUTPUT);
  digitalWrite(6, true);
}

// Move Slide Up Stop 
void SlideUpStop(void) {
  pinMode(6, OUTPUT);
  digitalWrite(6, false);
}

// Move Slide Down 
void SlideDown(void) {
  pinMode(7, OUTPUT);
  digitalWrite(7, true);
}

// Move Slide Down Stop 
void SlideDownStop(void) {
  pinMode(7, OUTPUT);
  digitalWrite(7, false);

// Write Servo Ref
//pinMode(27, OUTPUT);
//analogWrite(27,SpeedRef);
}



volatile unsigned int temp, counter = 0; //This variable will increase or decrease depending on the rotation of encoder
// Init Sharer and declare your function to share
void setup() {
	
//Encoder Setup
 pinMode(2, INPUT_PULLUP); // internal pullup input pin 2 
 pinMode(3, INPUT_PULLUP); // internalเป็น pullup input pin 3
//Setting up interrupt
  //A rising pulse from encodenren activated ai0(). AttachInterrupt 0 is DigitalPin nr 2 on moust Arduino.
  attachInterrupt(0, ai0, RISING);
  //B rising pulse from encodenren activated ai1(). AttachInterrupt 1 is DigitalPin nr 3 on moust Arduino.
  attachInterrupt(1, ai1, RISING);
//End Encoder Setup

Sharer.init(115200); // Init Serial communication with 115200 bauds

	// Share the built-in analogRead function
	Sharer_ShareFunction(int, analogRead, uint8_t, pin);

  // Share the built-in analogWrite function
  //Sharer_ShareFunction(int, analogWrite, uint8_t, pin);

	// Share the built-in digitalRead function
	Sharer_ShareFunction(bool, digitalRead, uint8_t, pin);

	// Expose this functions without returned parameters
	Sharer_ShareVoid(TurnLEDOn);
	Sharer_ShareVoid(TurnLEDOff);
  Sharer_ShareVoid(SlideUp);
  Sharer_ShareVoid(SlideUpStop);
  Sharer_ShareVoid(SlideDown);
  Sharer_ShareVoid(SlideDownStop);
  Sharer_ShareVoid(int, SpeedRef);
	
	// Share variables for read/write from desktop application
	Sharer_ShareVariable(int, counter);
  //Sharer_ShareVariable(int, SpeedRef);
	Sharer_ShareVariable(bool, a_boolean);
	Sharer_ShareVariable(long, a_long);
}

// Run Sharer engine in the main Loop
void loop() {
	Sharer.run();

	// If data available, just write back them
	int val = Sharer.read();
	if (val > 0) {
		Sharer.write(val);
	}

  pinMode(27, OUTPUT);
  analogWrite(27,500);
	
	a_long++; // ;)
//Encoder
// Send the value of counter
  if( counter != temp ){
  //Serial.println (counter);
  temp = counter;
  }
//End Encoder
}


void ai0() {
  // ai0 is activated if DigitalPin nr 2 is going from LOW to HIGH
  // Check pin 3 to determine the direction
  if(digitalRead(3)==LOW) {
  counter++;
  }else{
  counter--;
  }
  }
   
  void ai1() {
  // ai0 is activated if DigitalPin nr 3 is going from LOW to HIGH
  // Check with pin 2 to determine the direction
  if(digitalRead(2)==LOW) {
  counter--;
  }else{
  counter++;
  }
  }
