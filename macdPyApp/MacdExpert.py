import sys
from System import String
from time import sleep

#
# This MACD-based examples enters positions with stop and limit entry orders.
# During a timeUpdate() for the selected period
# if these conditions are met, position entry occurs:
# - Not currently in a position
# - 50 period EMA > 0
# - Current price is above 50 period Ema for buy, below for sell
# - macd and signal are above 0 for sell, below for buy
# - macd > signal and previous macd < previous signal for buy, reverse for sell
#

macdPrev = -1
signalPrev = -1
macdShort = 12
macdLong = 26
macdSignal = 9
emaPeriods = 50
amount = 1000
stopPips = 12
limitPips = 25

def Start():
	Factory.Display.appendLine( "Hello from Python for "+Pair)
	print Pair, TimeFrame

def Stop():
	print "Goodbye from Python"

def timeUpdate(timeFrame):
	global macdShort
	global macdPrev
	global signalPrev
	global macdLong
	global macdSignal
	global emaPeriods
	global amount
	global stopPips
	global limitPips
			
	lMacd = Base.getMACDasList(timeFrame, PriceComponent.BidClose, macdShort, macdLong, macdSignal)
	macd = lMacd[0]
	signal = lMacd[1]
	history = lMacd[2]
	Base.Log.debug(String.format("timeUpdate({0}):{1}, macd={2}, signal={3}",Base.Factory.TimeFrameMap[timeFrame], Pair,macd,signal))
	
	lastBid = Base.getLast(timeFrame, PriceComponent.BidClose)
	lastAsk = Base.getLast(timeFrame, PriceComponent.AskClose)
	ema = Base.getEMA(timeFrame, PriceComponent.BidClose, emaPeriods)


	if (not Base.InPosition
	and  ema > 0):

		if (macd < signal 
		and macdPrev > signalPrev
		and  macd > 0
		and signal > 0
		and lastBid < ema):
			entry = "SELL";
			Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, lastBid))
			Base.enterPosition(entry, amount, lastBid, stopPips, limitPips)

		elif (macd > signal
		and macdPrev < signalPrev
		and  macd > 0
		and signal > 0
		and lastAsk > ema):
			entry = "BUY";
			Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, lastAsk))
			Base.enterPosition(entry, amount, lastAsk, stopPips, limitPips)
		
	macdPrev = macd
	signalPrev = signal

def priceUpdate(datetime, bid, ask):
	if Factory.Debug:
		print String.Format("Python price update {0}: bid={1}, ask={2}",Pair,bid,ask);
	#Base.Log.debug(String.format("priceUpdate():{0}, bid={1}, ask={2}",Pair,bid,ask))

