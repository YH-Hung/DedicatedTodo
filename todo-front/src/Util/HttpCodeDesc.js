function HttpCodeDesc(code) {
    switch (code) {
        case 200:
            return "OK";
        case 201:
            return "Created";
        case 202:
            return "Accepted";
        case 204:
            return "No Content";
        case 400:
            return "Bad Request";
        case 404:
            return "Not Found";
        default:
            return "Not implemented code desc";
    }
}

export default HttpCodeDesc;
