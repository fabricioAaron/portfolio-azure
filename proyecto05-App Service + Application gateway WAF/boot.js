import http from 'k6/http';

export let options = {
  vus: 10,
  duration: '30s',
};

export default function () {
  const params = {
    headers: { 'User-Agent': 'BadBot/1.0' },
  };
  // REEMPLAZA CON TU IP
  http.get('http://4.177.16.203/', params);
}