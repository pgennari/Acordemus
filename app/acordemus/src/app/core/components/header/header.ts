import { Component } from '@angular/core';
import { CondoSelection } from "../condo-selection/condo-selection";
import { Sidebar } from "../sidebar/sidebar";

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrls: ['./header.css'],
  imports: [CondoSelection, Sidebar]
})
export class Header {
}
