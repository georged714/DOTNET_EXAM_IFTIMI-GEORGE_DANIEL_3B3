# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| R5 | BookingService.cs, linia 8, nu se verifică dacă startUtc e în trecut; TimeProvider nu e folosit nicăieri. | Citind codul | Injectez `TimeProvider` și returnez 400 dacă startUtc < now. |
| R6 | BookingService.cs, linia 8, nu există limită de 4 ore; o rezervare de oricâte ore e acceptată. | AI | Adaug verificare endUtc, startUtc <= 4h, altfel 400. |
| R7 | BookingService.cs, linia 26–30, ListForRoom returnează și rezervările anulate, care blochează intervalul; condiția <= pe ambele capete tratează intervalele care se ca suprapun. | AI | Filtrez doar Active, schimb condiția la < strict pe capete. |
| R8 | BookingService.cs, linia 56–60, Cancel() nu verifică dacă rezervarea e deja anulată, a doua anulare returnează 204 în loc de 409. | AI | Verific statusul înainte și returnez 409 dacă e deja Cancelled. |
