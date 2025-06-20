FROM golang:1.24

WORKDIR /usr/src/app

COPY ./udp_receiver.go /usr/src/app/udp_receiver.go

CMD ["go", "run", "udp_receiver.go"]

EXPOSE 12345/udp

