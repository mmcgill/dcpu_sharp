; Test echoing of keypresses to the display.

; loop until a key is pressed
	set i, 0				; cursor
	set b, 0x0				; color
:mainLoop
	jsr readKey
	ife a, 0xd				; carriage return
		set pc, hang
	
	ife a, 0
		set pc, doNotEcho

	bor a, b
	add b, 0x0100
	set [0x8000 + i], a
	add i, 1
	mod i, 384

:doNotEcho
	set pc, mainLoop

:hang
	sub pc,1

:readKey ; returns in a
	set push, i
	set i,[keypointer]
	add i,0x9000
	set a,[i]
	ife a,0
		set pc, readKeyEnd
	
	set [i],0
	add [keypointer], 1
	and [keypointer], 0xf

:readKeyEnd
	set i, pop
	set pc, pop ; return

:keypointer
dat 0

