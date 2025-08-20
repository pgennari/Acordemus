import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CondoSelection } from './condo-selection';

describe('CondoSelection', () => {
  let component: CondoSelection;
  let fixture: ComponentFixture<CondoSelection>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CondoSelection]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CondoSelection);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
