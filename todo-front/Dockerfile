FROM nginx:alpine

COPY ./nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY ./build .

ENTRYPOINT ["nginx", "-g", "daemon off;"]
