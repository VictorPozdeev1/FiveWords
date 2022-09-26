function fetchWithAuth(url, options = {}) {
    if (localStorage.jwtToken) {
        if (!options.headers)
            options.headers = {};
        options.headers.Authorization = `Bearer ${localStorage.jwtToken}`;
        return fetch(url, options);
    }
    else
        return Promise.reject();
}


function saveToken(token) {
    localStorage.setItem("jwtToken", token);
}

function removeToken() {
    localStorage.removeItem('jwtToken');
}

function isAuthenticated() {
    return !!localStorage.jwtToken;
}


export { fetchWithAuth, saveToken, removeToken, isAuthenticated }