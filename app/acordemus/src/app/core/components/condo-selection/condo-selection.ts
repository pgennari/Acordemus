import { Component } from '@angular/core';
import { CondoService } from './condo.service';
import { CommonModule } from '@angular/common';
import { Condo } from '../../../condo';

@Component({
  imports: [CommonModule],
  selector: 'app-condo-selection',
  templateUrl: './condo-selection.html',
  styleUrls: ['./condo-selection.css'],
})
export class CondoSelection {
  condos: Condo[] = [];

  constructor(private condoService: CondoService) {
    this.condos = this.condoService.getCondos();
  }

  get selectedCondo(): Condo | undefined {
    return this.condos.find((c) => c.selected);
  }

  get unselectedCondos(): Condo[] {
    return this.condos.filter((c) => !c.selected);
  }

  setSelectedCondo(condo: Condo) {
    this.condos.forEach((c) => (c.selected = false));
    condo.selected = true;
  }
}
