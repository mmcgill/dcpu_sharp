; Conway's Game of Life
; Renders a 64x64 field by writing characters
; on a 16x8 grid

ADD PC, 1
:randseed  
dat 0xACE1 ; change to get different initial states

; Initialize the screen
SET [0x8280], 0x4 ; red border color

; Set screen to the appropriate characters
SET A, 0xf000 ; white fg | black bg | char #

SET I, 0x8000
:loop_init
SET X, I
AND X, 0xf
SET Y, I
SHR Y, 1
AND Y, 0x70
BOR X, Y
BOR X, A
SET [I], X
ADD I, 1
IFN I, 0x8180
  SET PC, loop_init

; the internal grid is actually 66x66, to not
; have to check if an access is out-of-bounds
; (we set the border to do toroidal wrap-around)
:randomize_grid  ; set a random initial state
SET SP, 0x2105
:randomize_loop
JSR rand
AND A, 1
SET PUSH, A
IFN 0x0fff, SP
  SET PC, randomize_loop

; The core loop iterates over cells in a block pattern 
; it calculates 2x8 groups at a time, since that's
; the dimensions of one word of a character font

; C -- address of current field (since it's double-buffered)
; A, B -- coordinates inside current group
; X, Y -- coordinates of the current cell
; Z -- number of live neighbors
; SP -- address of last half-character we modified
; I -- top-left neighbor index
; J -- current half-character bitmap
;       we modify a character by doing SET PUSH, J

SET C, 0x1000 ; the live/dead cells are stored at 0x1000 and 0x3000

:loop_main
; copy cells to let us do toroidal wrap-around.
; we have an MxN matrix, and need to copy the 
; rows and columns to the opposite edges, and also
; do the corners properly

; Copy the M-1th row to the 1st row.
SET SP, C ; source
ADD SP, 0x1081 ; 66 * 64 + 1
SET I, C  ; target
SET X, I
ADD X, 65 ; the last element we write
:toroid_row_zero
ADD I, 1
SET [I], POP
IFN I, X
  SET PC, toroid_row_zero

; Copy the 2nd row to the Mth row.
SET SP, C ; source
ADD SP, 67
SET I, C  ; target
ADD I, 0x10c2 ; 66 * 65
SET X, I
ADD X, 65 ; the last element we write
:toroid_row_last
ADD I, 1
SET [I], POP
IFN I, X
  SET PC, toroid_row_last

; Do the columns.
SET I, C  ; left
SET J, C  ; right
ADD J, 64
SET X, I
ADD X, 0x1080 ; end address (X) (66 * 64)
SET A, 66   ; increment amount
:toroid_columns
ADD I, A
ADD J, A
SET [I], [J]
SET [J+1], [I+1]
IFN I, X
  SET PC, toroid_columns

; Do the corners.
SET [C], [C+0x10c0] ; (0,0) = (64,64)
SET [C+65], [C+0x1081] ; (65, 0) = (1, 64)
SET [C+0x10c2], [C+0x82] ; (0, 65) = (64, 1)
SET [C+0x1103], [C+67] ; (65, 65) = (1, 1)

SET X, 62 ; cell coords
SET Y, 56
SET SP, 0x8280 ; half-character address
:loop_group
SET A, 0
SET J, 0
  :loop_a
  SET B, 8

  SET I, Y ; I = (Y+A)*66 + (X+B) + C (index of top-left neighbor)
  BOR I, 7 ; hoisted out of the inner loop
  MUL I, 66
  ADD I, X
  ADD I, A
  ADD I, C

    :loop_b
    SUB B, 1

    ; count how many neighbors we have
    SET Z, [I]      ; -1, -1
    ADD Z, [I+0x1]  ;  0, -1
    ADD Z, [I+0x2]  ;  1, -1
    ADD Z, [I+0x42] ; -1,  0
    ADD Z, [I+0x44] ;  1,  0
    ADD Z, [I+0x84] ; -1,  1
    ADD Z, [I+0x85] ;  0,  1
    ADD Z, [I+0x86] ;  1,  1

    ; trick: cell is alive if (neighbors | alive) == 3
    BOR Z, [I+0x43]
    IFN Z, 3
      SET Z, 0
    AND Z, 1

    SHL J, 1
    IFE Z, 1
      XOR J, 1 ; set the font display

    XOR I, 0x4000 ; set the cell in the opposite page
    SET [I+0x43], Z
    XOR I, 0x4000

    SUB I, 66

    IFN B, 0
      SET PC, loop_b
  ADD A, 1
  IFN A, 2
    SET PC, loop_a
SET PUSH, J
SUB X, 2
IFN O, 0
  SET X, 62
IFN O, 0
  SUB Y, 8
IFG SP, 0x8180  ; have we written the last character?
  SET PC, loop_group

XOR C, 0x4000
SET PC, loop_main

:rand ; simple LFSR RNG -- only use the low bit!
  SET A, randseed
  SHR [A], 1
  IFN O, 0
     XOR [A], 0xB400
  SET A, [A]
  SET PC, POP
