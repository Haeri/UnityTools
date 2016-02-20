Instructions

Setup:
1. Drag and drop onto an UI-Text object inside a canvas.
2. Use the paragraph allignments on the text component, to allign the text.

Use:
1. Inside your Start() methode, call the static function requestID() to get an unique ID:
	int dBuggerID = DBugger.requestID(this);
2. Log your variables, by calling the static function log() and provide it with the unique ID:
   	DBugger.log("isAlive", isAlive, dBuggerID); 