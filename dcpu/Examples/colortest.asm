set i, 0	; index into video memory
set j, 0	; color
set z, 48	; character

:loop
	set a, j
	add j, 1
	and j, 0xff

	shl a, 8
	bor a, z
	add z, 1
	ifg z, 90
		set z, 48

	set [0x8000+i], a
	add i, 1
	ifn i, 384
		set pc, loop

:done
	set pc, done


