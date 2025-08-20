import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewProposal } from './new-proposal';

describe('NewProposal', () => {
  let component: NewProposal;
  let fixture: ComponentFixture<NewProposal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewProposal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewProposal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
