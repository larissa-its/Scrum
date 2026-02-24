set datafile separator ","
set key autotitle columnhead

set xdata time
set timefmt "%Y-%m-%d"
set format x "%b"
set xtics rotate by -45

set xrange ["2024-01-01":"2024-12-31"]

set title "Maximale Temperatur pro Tag"
set xlabel "Monat"
set ylabel "Temperatur (C)"

set style data lines

plot for [i=2:*] "dailymax.csv" using 1:i
pause -1
