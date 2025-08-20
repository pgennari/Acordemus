import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { Login } from "./components/login/login";
import { Header } from "./core/components/header/header";
// Importante: não precisa importar NewProposal aqui, pois será lazy loaded

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, MatSlideToggleModule, Login, Header],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected title = 'acordemus';
}
