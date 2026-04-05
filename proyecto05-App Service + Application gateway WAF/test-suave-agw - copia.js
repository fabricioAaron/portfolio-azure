import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
  vus: 5,
  duration: '30s',
};

export default function () {

  http.get('http://4.177.16.203/'); // REEMPLAZA CON TU IP-Pública del forntend del Aplication Gateway
}
  sleep(1);
}
