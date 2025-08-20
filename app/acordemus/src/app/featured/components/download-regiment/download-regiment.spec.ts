import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadRegiment } from './download-regiment';

describe('DownloadRegiment', () => {
  let component: DownloadRegiment;
  let fixture: ComponentFixture<DownloadRegiment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DownloadRegiment]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DownloadRegiment);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
