/* Cédric Vindevogel 1EL Project ICT */

#include <LiquidCrystal_I2C.h> // Bibliotheek voor display.
#include <OneWire.h> // Bibliotheek voor one wire connectie.
#include <DallasTemperature.h> // Bibliotheek voor de temperatuursensor. Temperatuursensor is van het type VMA324.
#include <DHT.h> // Bibliotheek voor de vochtigheidssensor. Vochtigheidssensor is van het type DHT11.
#include <Adafruit_BMP280.h> // Bibliotheek voor de drukksensor. Druksensor is van het type BMP280. Werkt via I2C.

#define ONE_WIRE_BUS 2 // De temperatuursensor op pin 2.
#define DHTPIN 6 // De vochtigheid sensor wordt verbonden met pin 6 van de Arduino.
#define DHTTYPE DHT11 // Het type van vochtigheid sensor die ik gebruik is DHT11.

OneWire oneWire(ONE_WIRE_BUS); // Start de onewire communicatie.
DallasTemperature sensors(&oneWire); // Zet de one wire communicatie om temperatuursensor.

DHT dht(DHTPIN, DHTTYPE); // Start de DHT communicatie.

Adafruit_BMP280 bmp; // I2C Interface voor druksensor. I2C adres is 0x77.

LiquidCrystal_I2C lcd = LiquidCrystal_I2C(0x27, 20, 4); // Type van I2C LCD display. I2C adres is 0x27. Display is 20 kolommen en 4 rijen groot.

void setup() {
  
  Serial.begin(9600); // Hiermee zet je de snelheid of de baudrate van de seriële data op 9600.

  sensors.begin(); // Starten van temperatuursensor bibliotheek.

  dht.begin(); // Starten van de vochtigheidssensor bibliotheek.

  bmp.begin(); // Starten van de druksensor bibliotheek.

  lcd.init(); // Starten van de lcd bibliotheek.
  lcd.backlight(); // Activeer de lcd backlight tijdens het hele programma.
}

void loop() {
  
  /* In dit stuk worden alle waarden ingelezen. */

  sensors.requestTemperatures(); // Lezen van de temperatuur.
  float vochtigheid = dht.readHumidity(); // Lezen van de luchtvochtigheid.
  float luchtdruk = bmp.readPressure()/100; // Meten van luchtdruk in hPa.

  /* In dit stuk wordt de string geprogrammeerd die serieel verstuurd zal worden. */

  Serial.println(String(sensors.getTempCByIndex(0)) + ';' + String(vochtigheid) + ';' + String(luchtdruk)); // Maak een string op een nieuwe lijn. Drie variabelen gescheiden door een punt komma.

  lcd.setCursor(5,0);
  lcd.print("Weerstation");
  lcd.setCursor(0,1);
  lcd.print("Temperatuur=");
  lcd.print((sensors.getTempCByIndex(0)));
  lcd.print("C");
  lcd.setCursor(0,2);
  lcd.print("Vochtigheid=");
  lcd.print(vochtigheid);
  lcd.print("%");
  lcd.setCursor(0,3);
  lcd.print("Luchtdruk=");
  lcd.print(luchtdruk);
  lcd.print("hPa");

  delay(1000); // Wacht 1s alvorens een nieuwe waarden te versturen.
  Serial.flush(); // Dit zorgt er voor dat zelfs na 1s seconde geen nieuwe lijn verstuurd zal er worden als de vorige nog niet verstuurd is.
}
