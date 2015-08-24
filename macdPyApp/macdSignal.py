import sys
from System import String
from time import sleep

macdPrev = -1
signalPrev = -1

def Start():
	Factory.Display.appendLine( "Hello from Python for "+Pair)
	print Pair, TimeFrame

def Stop():
	print "Goodbye from Python"

def timeUpdate(timeFrame):
	global macd
	global macdPrev
	global signalPrev
	if Factory.Debug:
		print "Update from Python"
		if Base.InPosition:
			Factory.Display.appendLine("Update for {0}, in position",Pair)
			
	lMacd = Base.getMACDasList(TimeFrame.m5, PriceComponent.BidClose, 12, 26, 9)
	print lMacd
	macd = lMacd[0]
	signal = lMacd[1]
	history = lMacd[2]
	Base.Log.debug(String.format("timeUpdate({0}):{1}, macd={2}, signal={3}",Base.Factory.TimeFrameMap[timeFrame], Pair,macd,signal))
	
	if (macd < signal 
	and macdPrev > signalPrev):
		Factory.Display.appendLine("SELL "+Pair)
		Base.Log.debug("SELL "+Pair)
	elif (macd > signal
	and macdPrev < signalPrev):
		Factory.Display.appendLine( "BUY "+Pair)
		Base.Log.debug("BUY "+Pair)
		
	macdPrev = macd
	signalPrev = signal

def priceUpdate(datetime, bid, ask):
	if Factory.Debug:
		print String.Format("Python price update {0}: bid={1}, ask={2}",Pair,bid,ask);
	#Base.Log.debug(String.format("priceUpdate():{0}, bid={1}, ask={2}",Pair,bid,ask))

