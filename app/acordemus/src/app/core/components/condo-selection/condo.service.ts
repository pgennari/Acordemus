import { Injectable } from '@angular/core';
import { Condo } from '../../../condo';

@Injectable({ providedIn: 'root' })
export class CondoService {
  getCondos(): Condo[] {
    return [
      { id: 1, name: 'Beira-Rio', shortName: 'Beira-Rio', selected: false },
      {
        id: 2,
        name: 'Residencial Fogaça',
        shortName: 'Fogaça',
        selected: false,
      },
      {
        id: 3,
        name: "Villas de Sant'ana",
        shortName: 'Villas',
        selected: false,
      },
      { id: 4, name: 'Mirante do Vale', shortName: 'Mirante', selected: false },
      { id: 5, name: 'Garden Family Club', shortName: 'GFC', selected: true },
    ];
  }
}
