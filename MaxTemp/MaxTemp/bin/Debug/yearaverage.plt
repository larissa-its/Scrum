set datafile separator ","

set title "Durchschnittstemperatur (an Tagen mit >50C)"
set xlabel "Uhrzeit"
set ylabel "Temperatur (C)"

set xrange [0:24]
set yrange [20:65]

set key outside
set grid

plot "yearaverage.csv" using 1:2 with lines lw 2 title "S1", \
     "" using 1:3 with lines lw 2 title "S2", \
     "" using 1:4 with lines lw 2 title "S3", \
     "" using 1:5 with lines lw 2 title "S4", \
     "" using 1:6 with lines lw 2 title "SB", \
     "" using 1:7 with lines lw 2 title "SD"