# Marten PoC

## What it does: 
PoCAPI sends a heartbeat every minute, this calls PoCEventRaiser which adds an event 
PoCEventHandler silently handles heartbeats

PoCAPI has a POST /message endpoint which expects a string.
PoCAPI does some simple validation, and then passes this on to PoCEventRaiser. (Start with H, has a space, ends in !)

PoCEventRaiser takes the message and splits each character into PocCharEvents
PoCEventHandler handles these, which it logs out the sequence Id and the Character, to show that they have been processed in order 

PoCEventHandler always returns highest sequence id of the events its written for the message endpoint

TODO: 
* PoCEventRaiser return sequence id to heartbeat
* Add checking the PoCEventHandler has processed the heartbeat
* Add another API, Raiser, Handler to show you can have multiple readers and writers
* Tests
* Azure build
* Deploy to k8s