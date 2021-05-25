# OpCC
Command-Line-Tool for Batch and Automation (Azure-DevOps Pipeline)

# Help

    opcc -?
    OpCC v:1.0.18.321.R

    OpCC -? .................... Display Help
    OpCC -start|-time|-stop .... TM - TimeMark, NCC - Norton Control Center
    OpCC -clear ................ Clear all EnvironmentVariables
    OpCC -wait:5 ............... Waits for 5 Seconds
    OpCC -display:Text -c:0C ... Display 'Text' in Color Red
    OpCC -display:Text -c:0A ... Display 'Text' in Color Green
                                   -c:BF the first corresponds to the background,
                                         the second the foreground
                                   0 = Black       8 = Gray
                                   1 = Blue        9 = Light Blue
                                   2 = Green       A = Light Green
                                   3 = Aqua        B = Light Aqua
                                   4 = Red         C = Light Red
                                   5 = Purple      D = Light Purple
                                   6 = Yellow      E = Light Yellow
                                   7 = White       F = Bright White

### TM - TimeMark (4.5) is a Tool from the Norton Utilities, (c) 1984-1988 Peter Norton
    tm start
    tm stop

Copyright 1984-1988, Peter Norton All rights reserved  
Standard Edition 4.50  
Tue 10/11/88  
(C) Copr 1984-88, Peter Norton

TM [START] [STOP] [Kommentar] [Zusätze]

Zusätze
* /Cn   Wählt Stoppuhr "n" aus ("n" ist 1, 2, 3 oder 4)
* /L    Schreibt TM-Informationen links auf den Monitor
* /LOG  Bereitet Informationsausgabe für Drucker oder Datei auf
* /N    Keine Einblendung aktueller Zeit oder Datum

TM-Time Mark

Keine Startzeit

### NCC - NortonControlCenter (6.1/5.1) is a Tool from the Norton Utilities, (c) 1991 Symantec Corporation
    ncc /start
    ncc /stop
